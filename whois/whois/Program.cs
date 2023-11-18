using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Cms;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System.ComponentModel.DataAnnotations;


namespace whois;
class Program
{
    static void Main(string[] args)
    {
        // if no arguments are entered into the static client
        if (args.Length == 0)
        {
            // ProcessCommand(Console.ReadLine());
            RunServer();
        }
        else
        {
            // print each argument inputted into the static client
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine(args[i]);
            }
        }

        // Process commands
        for (int i = 0; i < args.Length; i++)
        {

            ProcessCommand(args[i]);
        }
    }
    // set to true to see additional console outputs and information
    static Boolean debug = false;

    // create "conn" variable to allow connection to MySQL database
    static MySqlConnection conn = new MySqlConnection("server=localhost;user=root;database=DBNW;port=3306;password=L3tM31n");

    // InitUser will check if the inputted LoginID is present in the database, and if it is, output the associated UserID, if there are multiple UserID's connected to a LoginID, multiple returns true
    private static (string, string, Boolean) InitUser(string LoginID, string UserID, Boolean multiple)
    {
        try
        {
            // connect to database
            try
            {
                conn.Open();
            }
            catch(InvalidOperationException)
            {
                conn.Close();
                conn.Open();
            }

            // SQL query to find all UserID's in 'user-login-join' equal to the provided LoginID
            string logcheckqu = "SELECT UserID FROM `user-login-join` WHERE loginid = @loginput";

            // connect query to database
            MySqlCommand logCheck = new MySqlCommand(logcheckqu, conn);

            // insert inputted value into query
            logCheck.Parameters.AddWithValue("@loginput", LoginID);

            // collect values returned by query
            MySqlDataReader reader = logCheck.ExecuteReader();

            // if query returns results
            if (reader.HasRows)
            {
                // this list keeps track of which UserID's are returned by the query, it is in a list to allow room for selecting/modifying specific ID's, which is a feature I decided against implementing
                List<string> lUserIDs = new List<string>();

                //while the reader is receiving results, save each received result to the "lUserIDs" list
                while (reader.Read())
                {
                    lUserIDs.Add(reader.GetString(reader.GetOrdinal("UserID")));
                }

                // convert the list to an array so that ".length" can be used
                string[] aUserIDs = lUserIDs.ToArray();

                // if the LoginID is linked to multiple UserID's, return the inputted LoginID, null UserID and make "multiple" true
                if (aUserIDs.Length > 1)
                {
                    return (LoginID, null, true);
                }
                else
                {
                    // if there is only one UserID associated with the LoginID, return both and leave multiple as false
                    return (LoginID, aUserIDs[0], multiple);
                }
            }
            else
            {
                Console.WriteLine("\nLoginID '" + LoginID + "' not found");
                return (null, null, multiple);
            }
        }
        // if there are any errors when connecting to  or interacting with the database, output the error
        catch (Exception ex)
        {
            Console.WriteLine("\nAn error occured: " + ex);
            return (null, null, multiple);
        }
    }

    // LoadUser will load all of the information linked to the inputted UserID into class "User", which is then stored in a dictionary(key:LoginID, value:User())
    static void LoadUser(string uLoginID, string uUserID, Boolean multiple)
    {
        // Close and reopen database connection
        conn.Close();
        conn.Open();

        // if multiple UserIDs are connected to a LoginID, load empty data into the User instance
        if (multiple == true)
        {
            User.GetInstance().UserID = "Multiple UserID's are associated with this LoginID";
            User.GetInstance().Surname = "";
            User.GetInstance().Fornames = "";
            User.GetInstance().Title = "";
            User.GetInstance().Position = "";
            User.GetInstance().Phone = "";
            User.GetInstance().Email = "";
            User.GetInstance().Location = "Nowhere";

            User.GetUsers().Add(uLoginID, User.GetInstance());
        }

        // collect all data connected to the inputted UserID from the database and load it into a User class, then load into the dictionary
        else
        {
            string quLoadInfo = "SELECT * FROM UserInfo WHERE UserID = @uUserID";

            MySqlCommand LoadInfo = new MySqlCommand(quLoadInfo, conn);
            LoadInfo.Parameters.AddWithValue("@uUserID", uUserID);

            MySqlDataReader reader = LoadInfo.ExecuteReader();

            if (reader.Read())
            {
                User.GetInstance().UserID = reader.GetString("UserID");
                User.GetInstance().Surname = reader.GetString("Surname");
                User.GetInstance().Fornames = reader.GetString("fornames");
                User.GetInstance().Title = reader.GetString("Title");
                User.GetInstance().Position = reader.GetString("Position");
                User.GetInstance().Phone = reader.GetString("Phone");
                User.GetInstance().Email = reader.GetString("Email");
                User.GetInstance().Location = reader.GetString("Location");

                User.GetUsers().Add(uLoginID, User.GetInstance());
            }
            conn.Close();
        }


    }

    // Takes input from the static client and processes it in order to make database requests
    static void ProcessCommand(string command)
    {
        // Functions to process database requests

        // When an update request is made and the LoginID doesn't exist, create a new LoginID & UserID, leaving all fields as "unassigned" until they are updated
        void AddUser(String ID, String field, String update)
        {
            // check if LoginID contains prohibited special characters
            if (ID.Contains("(") || ID.Contains(")") || ID.Contains("'") || ID.Contains("[") || ID.Contains("]") || ID.Contains("&") || ID.Contains(" ") || ID.Contains("%") || ID.Contains("!") || ID.Contains("?") || ID.Contains("/") || ID.Contains("\""))
            {
                Console.WriteLine("\nLoginID cannot contain any whitespace, a slash \"/\" or any of the following meta-characters: \"()''[]&%!?\" including the quotes");

                Console.WriteLine("\nUser not added");
                return;
            }

            // Create new User and load to dictionary
            // Creates UserID as the LoginID with "user" added on the end (UserID will also not contain special characters due to check on LoginID)
            User.GetInstance().UserID = ID + "user";
            User.GetInstance().Surname = "unassigned";
            User.GetInstance().Fornames = "unassigned";
            User.GetInstance().Title = "unassigned";
            User.GetInstance().Position = "unassigned";
            User.GetInstance().Phone = "unassigned";
            User.GetInstance().Email = "unassigned";
            User.GetInstance().Location = "unassigned";

            User.GetUsers().Add(ID, User.GetInstance());

            try
            {
                conn.Open();

                // The newly created LoginID and UserID is added to the database
                string quNewUserInfo = $"INSERT INTO `UserInfo` (UserID, Surname, fornames, Title, Position, Phone, Email, Location) VALUES ('{User.GetInstance().UserID}','{User.GetInstance().Surname}','{User.GetInstance().Fornames}','{User.GetInstance().Title}','{User.GetInstance().Position}','{User.GetInstance().Phone}','{User.GetInstance().Email}','{User.GetInstance().Location}');";
                string quNewLogin = $"INSERT INTO `Login` (LoginID) VALUES ('{ID}');";
                string quNewUserLoginJoin = $"INSERT INTO `User-Login-Join` (UserID, LoginID) VALUES  ('{User.GetInstance().UserID}', '{ID}');";

                MySqlCommand cmNewUserInfo = new MySqlCommand(quNewUserInfo, conn);
                MySqlCommand cmNewLogin = new MySqlCommand(quNewLogin, conn);
                MySqlCommand cmNewUserLoginJoin = new MySqlCommand(quNewUserLoginJoin, conn);

                int rowsAffected1 = cmNewUserInfo.ExecuteNonQuery();
                int rowsAffected2 = cmNewLogin.ExecuteNonQuery();
                int rowsAffected3 = cmNewUserLoginJoin.ExecuteNonQuery();

                if (rowsAffected1 > 0 && rowsAffected2 > 0 && rowsAffected3 > 0)
                {
                    Console.WriteLine($"\nUserID : {ID} has been added");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred while updating the database: {ex}");
            }
            finally
            {
                conn.Close();
            }

            // Perform update on new user
            Update(ID, field, update);

            // output all values of newly created user
            Dump(ID);
        }
        // Delete LoginID
        void Delete(String ID)
        {
            if (debug) Console.WriteLine($"\n\nDelete record '{ID}' from Database");

            // Remove the user from the Dictionary
            User.GetUsers().Remove(ID);

            conn.Open();

            // Delete the LoginID and the connection of the LoginID to the UserID
            string qdelJoin = "DELETE FROM `user-login-join` WHERE LoginID = @reqID1";
            string qdelLog = "DELETE FROM `login` WHERE LoginID = @reqID2";

            MySqlCommand delJoin = new MySqlCommand(qdelJoin, conn);
            MySqlCommand delLog = new MySqlCommand(qdelLog, conn);


            delJoin.Parameters.AddWithValue("@reqID1", ID);
            delLog.Parameters.AddWithValue("@reqID2", ID);

            delJoin.ExecuteNonQuery();
            delLog.ExecuteNonQuery();

            conn.Close();

            Console.WriteLine($"\nLoginID : '{ID}' has been deleted");

        }
        // Output all fields associated with LoginID
        void Dump(String ID)
        {
            if (debug) Console.WriteLine("\n output all fields");
            Console.WriteLine($"\n\nUserID={User.GetUsers()[ID].UserID}");
            Console.WriteLine($"Surname={User.GetUsers()[ID].Surname}");
            Console.WriteLine($"Fornames={User.GetUsers()[ID].Fornames}");
            Console.WriteLine($"Title={User.GetUsers()[ID].Title}");
            Console.WriteLine($"Position={User.GetUsers()[ID].Position}");
            Console.WriteLine($"Phone={User.GetUsers()[ID].Phone}");
            Console.WriteLine($"Email={User.GetUsers()[ID].Email}");
            Console.WriteLine($"Location={User.GetUsers()[ID].Location}");
        }
        // Return one field from input
        void Lookup(String ID, String field)
        {
            if (debug)
                Console.WriteLine($"\n lookup field '{field}'");
            String result = null;
            switch (field)
            {
                case "location": result = User.GetUsers()[ID].Location; break;
                case "userid": result = User.GetUsers()[ID].UserID; break;
                case "surname": result = User.GetUsers()[ID].Surname; break;
                case "fornames": result = User.GetUsers()[ID].Fornames; break;
                case "title": result = User.GetUsers()[ID].Title; break;
                case "phone": result = User.GetUsers()[ID].Phone; break;
                case "position": result = User.GetUsers()[ID].Position; break;
                case "email": result = User.GetUsers()[ID].Email; break;
            }
            Console.WriteLine(result);
        }
        // Change value of field
        void Update(String ID, String field, String update)
        {
            // If a LoginID is associated with more than one UserID, values cannot be updated
            if (User.GetUsers()[ID].UserID == "Multiple UserID's are associated with this LoginID")
            {
                Console.WriteLine("\nLoginID is linked to multiple users, cannot be updated");
                return;
            }

            if (debug)
                Console.WriteLine($"\n update field '{field}' to '{update}'\n");

            // UserID is a primary key and therefor can't be changed
            if (field == "userid")
            {
                Console.WriteLine("\nUserID's cannot be changed");
                return;
            }

            // Phone numbers must be entered in a universal format
            if (field == "phone" && !(field.Length == 12 && field.StartsWith("+")))
            {
                Console.WriteLine("\nPhone numbers must be in the format +###########");
                return;
            }


            switch (field)
            {
                case "location": User.GetUsers()[ID].Location = update; break;
                case "userid": User.GetUsers()[ID].UserID = update; break;
                case "surname": User.GetUsers()[ID].Surname = update; break;
                case "fornames": User.GetUsers()[ID].Fornames = update; break;
                case "title": User.GetUsers()[ID].Title = update; break;
                case "phone": User.GetUsers()[ID].Phone = update; break;
                case "position": User.GetUsers()[ID].Position = update; break;
                case "email": User.GetUsers()[ID].Email = update; break;
            }


            try
            {
                conn.Open();

                // Send update to database
                string updateQuery = $"UPDATE UserInfo SET {field} = @updateValue WHERE UserID = @userID";
                MySqlCommand updateCommand = new MySqlCommand(updateQuery, conn);
                updateCommand.Parameters.AddWithValue("@updateValue", update);
                updateCommand.Parameters.AddWithValue("@userID", User.GetUsers()[ID].UserID);

                int rowsAffected = updateCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("\nUpdate successful in the database.");
                }
                else
                {
                    Console.WriteLine("\nNo rows affected. UserID not found in the database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred while updating the database: {ex}");
            }
            finally
            {
                conn.Close();
            }
        }

        Console.WriteLine($"\n\nCommand: {command}");

        try
        {
            // Split input where there is a question mark, making the first value the LoginID and assigning it to "ID"
            String[] slice = command.Split(new char[] { '?' }, 2);
            String ID = slice[0];
            String operation = null;
            String update = null;
            String field = null;

            // If there is a value after the question mark, assign that value to "operation", then split where there is an equals and assign the first value to "field"
            if (slice.Length == 2)
            {
                operation = slice[1].ToLower();
                String[] pieces = operation.Split(new char[] { '=' }, 2);
                field = pieces[0].ToLower();

                // If there is a value after the equals, assign that value to "update"
                if (pieces.Length == 2) update = pieces[1].ToLower();
            }

            // Check if LoginID has already been loaded
            if (!User.GetUsers().ContainsKey(ID))
                {
                     // Pass the LoginID to "InitUser" to determine if the LoginID exists, if it does, return the UserID or return true if there are multiple UserID's
                    var (uLoginID, uUserID, multiple) = InitUser(ID, null, false);
                    // Load the LoginID & UserID from "InitUser" into the Dictionary
                    LoadUser(uLoginID, uUserID, multiple);
                }

            if (debug)
            {
                // Check that the Dictionary has been created
                foreach (var kvp in User.GetUsers())
                {
                    Console.WriteLine($"\n\nKey: {kvp.Key}, Value: {kvp.Value}\n");
                }
            }

            // If the LoginID entered doesn't exist and the user is making an update request, create a new user with "AddUser" (i.e. "newuser?location=home)
            if (!User.GetUsers().ContainsKey(ID) && operation != null && field != null && update != null)
            {
                // If the user enters an invalid field, return an error (i.e. "newuser?salary=30000")
                if (!(field == "location" || field == "userid" || field == "surname" || field == "fornames" || field == "title" || field == "phone" || field == "position" || field == "email" || field == ""))
                {
                    Console.WriteLine($"\n\nField : '{field}' is not valid");
                    return;
                }
                else
                {
                    AddUser(ID, field, update);
                }

            }

            else if (User.GetUsers().ContainsKey(ID))
            {
                Console.Write($"\nOperation on ID '{ID}'");

                // If the user only enters a LoginID, return all fields (i.e. "cssbct")
                if (operation == null) Dump(ID);

                // If the user enters an invalid field, return an error (i.e. "cssbct?salary")
                else if (!(field == "location" || field == "userid" || field == "surname" || field == "fornames" || field == "title" || field == "phone" || field == "position" || field == "email" || field == ""))
                {
                    Console.WriteLine($"\n\nField : '{field}' is not valid");
                    return;
                }

                // If the user has a question mark after the LoginID, but leaves the field blank, delete the user (i.e. "cssbct?")
                else if (field == "") Delete(ID);
                // If the user enters a query with a valid field and LoginID, return the field value (i.e. "cssbct?location")
                else if (update == null) Lookup(ID, field);
                // If the user enters a valid LoginID & field and has a value after the equals, update the inputted field (i.e. "cssbct?location=home")
                else Update(ID, field, update);
            }
            else
            {
                Console.WriteLine("\nPlease enter a valid LoginID");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"\nFault in Command Processing: {e.ToString()}");
        }
    }

    static void doRequest(NetworkStream socketStream)
    {
        // Allows the program to read and write to the web
        StreamWriter sw = new StreamWriter(socketStream);
        StreamReader sr = new StreamReader(socketStream);

        if (debug) Console.WriteLine("\nWaiting for input from the client...");

        try
        {
            // Read input from web
            String line = sr.ReadLine();

            if (line == null)
            {
                if (debug) Console.WriteLine("\nIgnoring null command");
                return;
            }

            Console.WriteLine($"\nReceived Network Command: '{line}'");

            // If the client recieves an update request
            if (line == "POST / HTTP/1.1")
            {
                int content_length = 0;

                while (line != "")
                {
                    // Ignore lines starting with "content-length"
                    if (line.StartsWith("Content-Length: "))
                    {
                        content_length = Int32.Parse(line.Substring(16));
                    }
                    line = sr.ReadLine();
                    if (debug) Console.WriteLine($"\nSkipped Header Line: '{line}'");
                }

                line = "";
                // Read the command after all of the skipped "content-length"
                for (int i = 0; i < content_length; i++) line += (char)sr.Read();

                // The we have an update

                // sw.WriteLine(line);

                Console.WriteLine(line);

                // Web input will be split at the ampersand (name=<name>&location=<location>)
                String[] slices = line.Split(new char[] { '&' }, 2);
                // ID is taken afer 5 characters to account for "name="
                String ID = slices[0].Substring(5);
                // Value is taken after 9 characters to account for "location="
                String value = slices[1].Substring(9);

                if (slices.Length < 2 || !slices[0].Contains("name=") || !slices[1].Contains("location="))
                {
                    // This is an invalid request
                    sw.WriteLine("HTTP/1.1 400 Bad Request");
                    sw.WriteLine("Content-Type: text/plain");
                    sw.WriteLine();
                    sw.Flush();
                    Console.WriteLine($"\nUnrecognised command: '{line}'");
                    return;
                }

                if (debug) Console.WriteLine($"\nReceived an update request for '{ID}' to '{value}'");

                // The web input to put in "command" format and then passed through "ProcessCommand"
                // The web client is only able to query location
                string command = $"{ID}?location={value}";
                ProcessCommand(command);

                // sw.WriteLine("HTTP/1.1 200 OK");
                // sw.WriteLine("Content-Type: text/plain");
                // sw.WriteLine();
                sw.WriteLine($"{ID} location is now {value}");
                sw.Flush();
            }

            else if (line.StartsWith("GET /?name=") && line.EndsWith(" HTTP/1.1"))
            {
                // then we have a lookup
                if (debug) Console.WriteLine("\nReceived a lookup request");
                String[] slices = line.Split(" ");  // Split into 3 pieces
                String ID = slices[1].Substring(7);  // start at the 7th letter of the middle slice - skip `/?name=`

                // LoginID from web input is initialised and loaded into the Dictionary
                if (!User.GetUsers().ContainsKey(ID))
                {
                    var (uLoginID, uUserID, multiple) = InitUser(ID, null, false);
                    LoadUser(uLoginID, uUserID, multiple);
                }

                // If the LoginID exists, return location
                if (User.GetUsers().ContainsKey(ID))
                {
                    String result = User.GetUsers()[ID].Location;

                    sw.WriteLine("HTTP/1.1 200 OK");
                    sw.WriteLine("Content-Type: text/plain");
                    sw.WriteLine();
                    sw.WriteLine(result);
                    sw.Flush();
                    Console.WriteLine($"\nPerformed Lookup on '{ID}' returning '{result}'");
                }
                else //return error
                {
                    sw.WriteLine("HTTP/1.1 404 Not Found");
                    sw.WriteLine("Content-Type: text/plain");
                    sw.WriteLine();
                    sw.Flush();
                    Console.WriteLine($"\nPerformed Lookup on '{ID}' returning '404 Not Found'");
                }
            }

            else
            {
                // We have an error
                Console.WriteLine($"\nUnrecognised command: '{line}'");
                sw.WriteLine("HTTP/1.1 400 Bad Request");
                sw.WriteLine("Content-Type: text/plain");
                sw.WriteLine();
                sw.Flush();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"\nFault in Command Processing: {e.ToString()}");
        }
        finally
        {
            if (debug) Console.WriteLine("\ndoRequest Done");
            sw.Close();
            sr.Close();
        }

    }

    static void RunServer()
    {
        // Listen for web inputs from port 43
        TcpListener listener = new TcpListener(43); ;
        Socket connection;

        try
        {
            listener.Start();
            if (debug) Console.WriteLine("\nServer Waiting connection...");
            while (true)
            {
                // Make connection upon receiving data
                connection = listener.AcceptSocket();

                // Create a new thread for each connection to allow for multithreading
                Thread thread = new Thread(() => initThread(connection));
                thread.Start();
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            listener.Stop();
        }

        if (debug)
            Console.WriteLine("\nTerminating Server");
    }

    static void initThread(Socket connection)
    {
        {
            NetworkStream socketStream = new NetworkStream(connection);
            try
            {
                // Allow 1000ms for connection, then close
                connection.SendTimeout = 1000;
                connection.ReceiveTimeout = 1000;
                // Send information to "doRequest"
                doRequest(socketStream);
            }
            catch (IOException)
            {
                Console.WriteLine("\nTimeout");
            }
            finally
            {
                connection.Close();
                socketStream.Close();
            }
        }
    }
}