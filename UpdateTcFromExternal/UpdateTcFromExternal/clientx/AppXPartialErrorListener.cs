//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================

using System;

using Teamcenter.Soa.Client.Model;


namespace Teamcenter.ClientX
{

/**
 * Implementation of the PartialErrorListener. Print out any partial errors
 * returned.
 *
 */
public class AppXPartialErrorListener : PartialErrorListener
{

    /*
     * (non-Javadoc)
     *
     * @see com.teamcenter.soa.client.model.PartialErrorListener#handlePartialError(com.teamcenter.soa.client.model.ErrorStack[])
     */
    public void HandlePartialError(ErrorStack[] stacks)
    {
        if (stacks.Length == 0) return;

        Console.WriteLine("");
        Console.WriteLine("*****");
        Console.WriteLine("Partial Errors caught in com.teamcenter.clientx.AppXPartialErrorListener.");


        for (int i = 0; i < stacks.Length; i++)
        {
            ErrorValue[] errors = stacks[i].ErrorValues;
            Console.Write("Partial Error for ");

            // The different service implementation may optionally associate
            // an ModelObject, client ID, or nothing, with each partial error
            if (stacks[i].HasAssociatedObject() )
            {
                Console.WriteLine("object "+ stacks[i].AssociatedObject.Uid);
            }
            else if (stacks[i].HasClientId())
            {
                Console.WriteLine("client id "+ stacks[i].ClientId);
            }
            else if (stacks[i].HasClientIndex())
            {
                Console.WriteLine("client index " + stacks[i].ClientIndex);
            }

            // Each Partial Error will have one or more contributing error messages
            for (int j = 0; j < errors.Length; j++)
            {
                Console.WriteLine("    Code: " + errors[j].Code + "\tSeverity: "
                        + errors[j].Level + "\t" + errors[j].Message);
            }
        }

    }

}
}
