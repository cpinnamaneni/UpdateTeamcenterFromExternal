//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================



using System;
using System.IO;

using Teamcenter.Schemas.Soa._2006_03.Exceptions;
using Teamcenter.Soa;
using Teamcenter.Soa.Common;
using Teamcenter.Soa.Client;
using Teamcenter.Soa.Exceptions;

namespace Teamcenter.ClientX
{

/**
 * The CredentialManager is used by the Teamcenter Services framework to get the
 * user's credentials when challanged by the server. This can occur after a period
 * of inactivity and the server has timed-out the user's session, at which time
 * the client application will need to re-authenitcate. The framework will
 * call one of the getCredentials methods (depending on circumstances) and will
 * send the SessionService.login service request. Upon successfull completion of
 * the login service request. The last service request (one that cuased the challange) 
 * will be resent.
 * 
 * The framework will also call the setUserPassword setGroupRole methods when ever
 * these credentials change, thus allowing this implementation of the CredentialManager
 * to cache these values so prompting of the user is not requried for  re-authentication.
 * 
 */
public class AppXCredentialManager : CredentialManager
{

    private String name          = null;
    private String password      = null;
    private String group         = "";          // default group
    private String role          = "";          // default role
    private String discriminator = "SoaAppX";   // always connect same user
                                                // to same instance of server

    /**
     * Return the type of credentials this implementation provides, 
     * standard (user/password) or Single-Sign-On. In this case
     * Standard credentials are returned.
     * 
     * @see com.teamcenter.soa.client.CredentialManager#getCredentialType()
     */
    public int CredentialType
    {
        get { return SoaConstants.CLIENT_CREDENTIAL_TYPE_STD; }
    }

    /**
     * Prompt's the user for credentials.
     * This method will only be called by the framework when a login attempt has
     * failed.
     * 
     * @see com.teamcenter.soa.client.CredentialManager#getCredentials(com.teamcenter.schemas.soa._2006_03.exceptions.InvalidCredentialsException)
     */
    public string[] GetCredentials(InvalidCredentialsException e)
    //throws CanceledOperationException
    {
        Console.WriteLine(e.Message);
        return PromptForCredentials();
    }

    /**
     * Return the cached credentials.
     * This method will be called when a service request is sent without a valid
     * session ( session has expired on the server).
     * 
     * @see com.teamcenter.soa.client.CredentialManager#getCredentials(com.teamcenter.schemas.soa._2006_03.exceptions.InvalidUserException)
     */
    public String[] GetCredentials(InvalidUserException e) 
    //throws CanceledOperationException
    {
        // Have not logged in yet, shoult not happen but just in case
        if (name == null) return PromptForCredentials();

        // Return cached credentials
        String[] tokens = { name, password, group, role, discriminator };
        return tokens;
    }

    /**
     * Cache the group and role
     * This is called after the SessionService.setSessionGroupMember service 
     * operation is called.
     * 
     * @see com.teamcenter.soa.client.CredentialManager#setGroupRole(java.lang.String,
     *      java.lang.String)
     */
    public void SetGroupRole(String group, String role)
    {
        this.group = group;
        this.role = role;
    }

    /**
     * Cache the User and Password
     * This is called after the SessionService.login service operation is called.
     * 
     * @see com.teamcenter.soa.client.CredentialManager#setUserPassword(java.lang.String,
     *      java.lang.String, java.lang.String)
     */
    public void SetUserPassword(String user, String password, String discriminator)
    {
        this.name = user;
        this.password = password;
        this.discriminator = discriminator;
    }

    
    public String[] PromptForCredentials() 
    //throws CanceledOperationException
    {
        try
        {
            Console.WriteLine("Please enter user credentials (return to quit):");
            Console.Write("User Name: ");
            name = Console.ReadLine();

            if (name.Length == 0) 
                throw new CanceledOperationException("");

            Console.Write("Password:  ");
            password = Console.ReadLine();
        }
        catch (IOException e)
        {
            String message = "Failed to get the name and password.\n" + e.Message;
            Console.WriteLine(message);
            throw new CanceledOperationException(message);
        }

        String[] tokens = { name, password, group, role, discriminator };
        return tokens;
    }

}
}
