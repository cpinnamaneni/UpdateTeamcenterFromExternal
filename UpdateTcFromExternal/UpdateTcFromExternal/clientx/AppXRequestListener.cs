//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================

using System;

using Teamcenter.Soa.Client;


namespace Teamcenter.ClientX
{

    /**
     * This implemenation of the RequestListener, logs each service request
     * to the console.
     *
     */
    public class AppXRequestListener : RequestListener
    {


        /**
         * Called before each request is sent to the server.
         */
        public void ServiceRequest(ServiceInfo info)
        {
            // will log the service name when done
        }

        /**
         * Called after each response from the server.
         * Log the service operation to the console.
         */
        public void ServiceResponse(ServiceInfo info)
        {
            Console.WriteLine(info.Id + ": " + info.Service + "." + info.Operation);
        }

    }
}
