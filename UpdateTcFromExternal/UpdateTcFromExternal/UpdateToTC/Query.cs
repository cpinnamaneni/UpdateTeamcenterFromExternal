//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================

using System;

using Teamcenter.ClientX;
using Teamcenter.Schemas.Soa._2006_03.Exceptions;

//Include the Saved Query Service Interface
using Teamcenter.Services.Strong.Query;

// Input and output structures for the service operations 
// Note: the different namespace from the service interface
using Teamcenter.Services.Strong.Query._2006_03.SavedQuery;

using Teamcenter.Services.Strong.Core;
using Teamcenter.Soa.Client.Model;

using ImanQuery = Teamcenter.Soa.Client.Model.Strong.ImanQuery;
using SavedQueriesResponse = Teamcenter.Services.Strong.Query._2007_09.SavedQuery.SavedQueriesResponse;
using QueryInput           = Teamcenter.Services.Strong.Query._2008_06.SavedQuery.QueryInput;
using QueryResults         = Teamcenter.Services.Strong.Query._2007_09.SavedQuery.QueryResults;


namespace Teamcenter.Hello
{
public class Query
{

    /**
     * Perform a simple query of the database
     * 
     */
    public void queryItems()
    {

        ImanQuery query = null;

        // Get the service stub
        SavedQueryService queryService  = SavedQueryService.getService(Session.getConnection());
        DataManagementService dmService = DataManagementService.getService(Session.getConnection());

        try
        {
 
            // *****************************
            // Execute the service operation
            // *****************************
            GetSavedQueriesResponse savedQueries = queryService.GetSavedQueries();
            
            
            if (savedQueries.Queries.Length == 0)
            {
                Console.Out.WriteLine("There are no saved queries in the system.");
                return;
            }

            // Find one called 'Item Name'
            for (int i = 0; i < savedQueries.Queries.Length; i++)
            {

                if (savedQueries.Queries[i].Name.Equals("Item Name"))
                {
                    query = savedQueries.Queries[i].Query;
                    break;
                }
            }
        }
        catch (ServiceException e)
        {
            Console.Out.WriteLine("GetSavedQueries service request failed.");
            Console.Out.WriteLine(e.Message);
            return;
        }

        if (query == null)
        {
            Console.WriteLine("There is not an 'Item Name' query.");
            return;
        }

        try
        {
            // Search for all Items, returning a maximum of 25 objects
            QueryInput[] savedQueryInput = new QueryInput[1];
            savedQueryInput[0] = new QueryInput();
            savedQueryInput[0].Query = query;
            savedQueryInput[0].MaxNumToReturn = 25;
            savedQueryInput[0].LimitList = new Teamcenter.Soa.Client.Model.ModelObject[0];
            savedQueryInput[0].Entries = new String[] { "Item Name" };
            savedQueryInput[0].Values = new String[1];
            savedQueryInput[0].Values[0] = "*";
           
            //*****************************
            //Execute the service operation
            //*****************************
            SavedQueriesResponse savedQueryResult = queryService.ExecuteSavedQueries(savedQueryInput);
            QueryResults found = savedQueryResult.ArrayOfResults[0]; 
           
                    
            System.Console.Out.WriteLine("");
            System.Console.Out.WriteLine("Found Items:");
            // Page through the results 10 at a time
            for (int i = 0; i < found.ObjectUIDS.Length; i += 10)
            {
                int pageSize = (i + 10 < found.ObjectUIDS.Length) ? 10 : found.ObjectUIDS.Length - i;

                String[] uids = new String[pageSize];
                for (int j = 0; j < pageSize; j++)
                {
                    uids[j] = found.ObjectUIDS[i + j];
                }
                ServiceData sd = dmService.LoadObjects(uids);
                ModelObject[] foundObjs = new ModelObject[sd.sizeOfPlainObjects()];
                for (int k = 0; k < sd.sizeOfPlainObjects(); k++)
                {
                    foundObjs[k] = sd.GetPlainObject(k);
                }

                Session.printObjects(foundObjs);
            }
        }
        catch (ServiceException e)
        {
            Console.Out.WriteLine("ExecuteSavedQuery service request failed.");
            Console.Out.WriteLine(e.Message);
            return;
        }

    }
}
}
