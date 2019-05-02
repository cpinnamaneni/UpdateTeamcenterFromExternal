//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================

using System;

using Teamcenter.Soa.Client.Model;
using Teamcenter.Soa.Exceptions;

namespace Teamcenter.ClientX
{


/**
 * Implementation of the ChangeListener. Print out all objects that have been updated.
 * 
 */
    public class AppXModelEventListener : ModelEventListener
    {

        override public void LocalObjectChange(ModelObject[] objects)
        {
            if (objects.Length == 0) return;
            System.Console.WriteLine("");
            System.Console.WriteLine("Modified Objects handled in com.teamcenter.clientx.AppXUpdateObjectListener.modelObjectChange");
            System.Console.WriteLine("The following objects have been updated in the client data model:");
            for (int i = 0; i < objects.Length; i++)
            {
                String uid = objects[i].Uid;
                String type = objects[i].GetType().Name;
                String name = "";
                if (objects[i].GetType().Name.Equals("WorkspaceObject"))
                {
                    ModelObject wo = objects[i];
                    try
                    {
                        name = wo.GetProperty("object_string").StringValue;
                    }
                    catch (NotLoadedException /*e*/) {} // just ignore
                }
                System.Console.WriteLine("    " + uid + " " + type + " " + name);
            }
        }


        override public void LocalObjectDelete(string[] uids)
        {
            if (uids.Length == 0)
                return;

            System.Console.WriteLine("");
            System.Console.WriteLine("Deleted Objects handled in com.teamcenter.clientx.AppXDeletedObjectListener.modelObjectDelete");
            System.Console.WriteLine("The following objects have been deleted from the server and removed from the client data model:");
            for (int i = 0; i < uids.Length; i++)
            {
                System.Console.WriteLine("    " + uids[i]);
            }

        }


    }
}
