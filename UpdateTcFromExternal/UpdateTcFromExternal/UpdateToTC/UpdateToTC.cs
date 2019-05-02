using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Teamcenter.ClientX;
using Teamcenter.Services.Strong.Core;
using Teamcenter.Services.Strong.Query;
//using Teamcenter.Soa.Client.Model.Strong.User;
using User = Teamcenter.Soa.Client.Model.Strong.User;
using Item = Teamcenter.Soa.Client.Model.Strong.Item;
using ItemRevision = Teamcenter.Soa.Client.Model.Strong.ItemRevision;


///using Teamcenter.Services.Strong.Core._2007_01.DataManagement;
using Teamcenter.Soa.Client.Model.Strong;
using Teamcenter.Soa.Client.Model;
using Teamcenter.Services.Strong.Core._2007_01.Session;
using Teamcenter.Services.Strong.Core._2006_03.FileManagement;
using System.Collections;
using Teamcenter.Services.Strong.Core._2007_12.Session;
using Teamcenter.Services.Strong.Core._2010_04.Session;

using Teamcenter.Services.Strong.Query._2006_03.SavedQuery;
using Teamcenter.Schemas.Soa._2006_03.Exceptions;

using ImanQuery = Teamcenter.Soa.Client.Model.Strong.ImanQuery;
using SavedQueriesResponse = Teamcenter.Services.Strong.Query._2007_09.SavedQuery.SavedQueriesResponse;
using QueryInput = Teamcenter.Services.Strong.Query._2008_06.SavedQuery.QueryInput;
using QueryResults = Teamcenter.Services.Strong.Query._2007_09.SavedQuery.QueryResults;
using Teamcenter.Services.Strong.Query._2010_04.SavedQuery;
using Teamcenter.Services.Strong.Core._2010_09.DataManagement;
using Teamcenter.Services.Strong.Core._2008_06.DataManagement;
using Teamcenter.Services.Strong.Core._2007_01.DataManagement;
using Teamcenter.Services.Strong.Workflow._2008_06.Workflow;
using System.Globalization;

namespace updateTcFromExternal
{


    public class UpdateToTC
    {
        public static String userid = null;
        public static String password = null;
        public static String group = null;
        public static String role = "";
        public static String volume = "";

        public static String serverHost = null;
        public static Teamcenter.ClientX.Session session;
        public static DataManagementService dmService;
        public static SessionService sessionService;
        public static SavedQueryService queryService;
        public static FileManagementService fileMgtService;

        //public static String localUserId = null;
        //public static String localPasswd = null;
        //public static String localGroup = null;
        //public static String localServerHost = null;

        //public static String actionStr = null;
        //public static String outputDir = null;

        public static User user = null;

        public  UpdateToTC(String tcUserid, String tcPassword, String tcGroup, String tcServerHost)
        {
            userid = tcUserid;
            password = tcPassword;
            group = tcGroup;
            serverHost = tcServerHost;
            
        }

        public static void InitializeProces()
        {
            initialize();
            user = login();
            byPassPrivileges();
        }


        public static void initialize()
        {
            session = new Teamcenter.ClientX.Session(serverHost);

            dmService = DataManagementService.getService(Teamcenter.ClientX.Session.getConnection());
            //prefService = PreferenceManagementService.getService(Session.getConnection());
            sessionService = SessionService.getService(Teamcenter.ClientX.Session.getConnection());
            queryService = SavedQueryService.getService(Teamcenter.ClientX.Session.getConnection());
            fileMgtService = FileManagementService.getService(Teamcenter.ClientX.Session.getConnection());
        }

        public static User login()
        {
            return session.login(userid, password, group, role);
            //return session.login();
        }

        public static void logout()
        {
            session.logout();
        }

        public static void byPassPrivileges()
        {
            try
            {
                GetTCSessionInfoResponse sessionInfoResponse = sessionService.GetTCSessionInfo();
                if (sessionInfoResponse != null)
                {
                    if (!(sessionInfoResponse.Bypass))
                    {
                        System.Console.WriteLine("Bypass is not set.....");
                        System.Console.WriteLine("Bypassing the privileges....");

                        StateNameValue[] stateName = new StateNameValue[1];
                        stateName[0] = new StateNameValue();
                        //stateName[0].setName("bypassFlag");
                        //stateName[0].setValue("true");

                        stateName[0].Name = "bypassFlag";
                        stateName[0].Value = "true";
                        sessionService.SetUserSessionState(stateName);

                        sessionInfoResponse = sessionService.GetTCSessionInfo();
                        if (sessionInfoResponse.Bypass)
                            System.Console.WriteLine("Successfully set the bypass...");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }


        //public void CreateObjectinTC(CreateObjectParameters createObjectPar)
        //{

        //    //UpdateInTC UpdateInTCObj = new UpdateInTC("georpras", "georpras", "", "http://detmscplmdev08.magna.global:8080/tc");
        //    InitializeProces();

        //    createObject();
        //}

        public static void printonConcole(String Text)
        {
            Console.WriteLine(Text);
        }

        public static CreateObjectOutput CreateObjectinTCAndReturnTCItemIdAndRev(CreateObjectParameters[] createObjectParList,String ItemTypeName,String ItemRevisionName)
        {

            String item_id = "";
            String revision_id = "";

            InitializeProces();
            // The create input for the ChangeNotice Item
            Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateInput itemCreateIn = new Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateInput();
            itemCreateIn.BoName = ItemTypeName;
            //itemCreateIn.BoName = "A9_AutoCN";

            // The create input for the ChangeNoticeRevision
            Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateInput revisionCreateIn = new Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateInput();
            revisionCreateIn.BoName = ItemRevisionName;
            //revisionCreateIn.BoName = "A9_AutoCNRevision";

            foreach (CreateObjectParameters createObjectPar in createObjectParList)
            {
               
                String propType = createObjectPar.propType;
                String propOn = createObjectPar.propOn;

                switch(propType.ToLower())
                {
                    case "string":
                        if(propOn == "Item")
                        {
                            itemCreateIn.StringProps.Add(createObjectPar.propName, createObjectPar.propValue);
                        }
                        else
                        {
                            revisionCreateIn.StringProps.Add(createObjectPar.propName, createObjectPar.propValue);
                        }
                        break;
                    case "date":
                        DateTime dateValue = Teamcenter.Soa.Client.Model.Property.ParseDate(createObjectPar.propValue);
                        if (propOn == "Item")
                        {
                            itemCreateIn.DateProps.Add(createObjectPar.propName, dateValue);
                        }
                        else
                        {
                            revisionCreateIn.DateProps.Add(createObjectPar.propName, dateValue);
                        }
                        break;

                }

            }

            // Tie the Revision CreateInput to the Item CreateInput
            itemCreateIn.CompoundCreateInput.Add("revision", new Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateInput[] { revisionCreateIn });

            // The data for the createObjects call
            Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateIn cnCreateIn = new Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateIn();
            cnCreateIn.ClientId = "One";
            cnCreateIn.Data = itemCreateIn;

            CreateResponse createResponse = dmService.CreateObjects(new Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateIn[] { cnCreateIn });
            if (createResponse.ServiceData.sizeOfPartialErrors() > 0)
            {
                //createResponse.ServiceData.;
            }
            else
            {

                //Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateOut[] createOut = createResponse.Output;
                foreach (Teamcenter.Services.Strong.Core._2008_06.DataManagement.CreateOut createOut in createResponse.Output)
                {
                    //createOut.ClientId
                    Console.WriteLine("Response for client ID " + createOut.ClientId);
                    foreach (ModelObject modelObject in createOut.Objects)
                    {
                        //modelObject.
                        //Console.WriteLine();
                        String[] uids = new String[1];
                        uids[0] = modelObject.Uid;
                        ServiceData sd = dmService.LoadObjects(uids);

                        ModelObject[] foundObjs = new ModelObject[sd.sizeOfPlainObjects()];
                        for (int k = 0; k < sd.sizeOfPlainObjects(); k++)
                        {
                            foundObjs[k] = sd.GetPlainObject(k);
                            
                            if (foundObjs[k].SoaType.ClassName == ItemRevisionName)
                            {
                                dmService.GetProperties(foundObjs, new String[] { "item_id" });
                                item_id = foundObjs[k].GetPropertyDisplayableValue("item_id");

                                dmService.GetProperties(foundObjs, new String[] { "item_revision_id" });
                                revision_id = foundObjs[k].GetPropertyDisplayableValue("item_revision_id");                                
                            }
                        }
                    }
                }
            }

            CreateObjectOutput createObjectOutput = new CreateObjectOutput(item_id, revision_id);

            return createObjectOutput;
        }

        //public void AutoPromoteWF()
        //{
        //    Workflow 
        //}
    }

    public class CreateObjectParameters
    {
        public String propName = null;
        public String propType = null;
        public String propValue = null;
        public String propOn = null;

        public CreateObjectParameters(String _propName, String _propType,String _propValue, String _propOn)
        {
            propName = _propName;
            propType = _propType;
            propValue = _propValue;
            propOn = _propOn;
        }
    }

    public class CreateObjectOutput
    {
        public String item_id = null;
        public String revision_id = null;

        public CreateObjectOutput(String _item_id, String _revision_id)
        {
            item_id = _item_id;
            revision_id = _revision_id;
        }
    }
}
