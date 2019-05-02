//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================


using System;
using System.IO;

using Teamcenter.Schemas.Soa._2006_03.Exceptions;
using Teamcenter.Soa.Client;
using Teamcenter.Soa.Exceptions;

namespace Teamcenter.ClientX
{


    /**
     * Implementation of the ExceptionHandler. For ConnectionExceptions (server
     * temporarily down .etc) prompts the user to retry the last request. For other
     * exceptions convert to a RunTime exception.
     */
    public class AppXExceptionHandler : ExceptionHandler
    {

        /*
         * (non-Javadoc)
         * 
         * @see com.teamcenter.soa.client.ExceptionHandler#handleException(com.teamcenter.schemas.soa._2006_03.exceptions.InternalServerException)
         */
        public void HandleException(InternalServerException ise)
    {
        Console.WriteLine("");
        Console.WriteLine("*****");
        Console.WriteLine("Exception caught in com.teamcenter.clientx.AppXExceptionHandler.handleException(InternalServerException).");


        if (ise is ConnectionException)
        {
            // ConnectionException are typically due to a network error (server
            // down .etc) and can be recovered from (the last request can be sent again,
            // after the problem is corrected).
            Console.Write("\nThe server returned an connection error.\n" + ise.Message
                           + "\nDo you wish to retry the last service request?[y/n]");
        }
        else if (ise is ProtocolException)
        {
            // ProtocolException are typically due to programming errors
            // (content of HTTP
            // request is incorrect). These are generally can not be
            // recovered from.
            Console.Write("\nThe server returned an protocol error.\n" + ise.Message
                           + "\nThis is most likely the result of a programming error."
                           + "\nDo you wish to retry the last service request?[y/n]");
        }
        else
        {
            Console.WriteLine("\nThe server returned an internal server error.\n"
                             + ise.Message
                             + "\nThis is most likely the result of a programming error."
                             + "\nA RuntimeException will be thrown.");
            throw new SystemException(ise.Message);
        }

        try
        {
            String retry = Console.ReadLine();
            // If yes, return to the calling SOA client framework, where the
            // last service request will be resent.
            if (retry.ToLower().Equals("y") || retry.ToLower().Equals("yes")) 
                return;

            throw new SystemException("The user has opted not to retry the last request");
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Failed to read user response.\nA RuntimeException will be thrown.");
            throw new SystemException(e.Message);
        }
    }

        /*
         * (non-Javadoc)
         * 
         * @see com.teamcenter.soa.client.ExceptionHandler#handleException(com.teamcenter.soa.exceptions.CanceledOperationException)
         */
        public void HandleException(CanceledOperationException coe)
    {
        Console.WriteLine("");
        Console.WriteLine("*****");
        Console.WriteLine("Exception caught in com.teamcenter.clientx.AppXExceptionHandler.handleException(CanceledOperationException).");

        // Expecting this from the login tests with bad credentials, and the
        // AnyUserCredentials class not
        // prompting for different credentials
        throw new SystemException(coe.Message);
    }

    }
}
