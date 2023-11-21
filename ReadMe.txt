Whois Program - by Archie Hull

#####################
Initialising database
#####################


DBNW_init.sql contains the SQL code required to create the "DBNW" database.
DBNW_load.sql contains the SQL code required to load user information into the database.

DBNW_load.sql will output all tables contained in the database, providing a concise view of queryable information.

Run both files in an SQL environment to create the database and table.



#####################
 whois static client
#####################


# Running whois #


Navigate to the file location of whois.exe in the commandline and enter the following:

"[filepath]> whois [command]" 

or

"[filepath]> ./whois [command]" 



# Commands #

1	:	[LoginID] 			: Returns all fields		

2	:	[LoginID]?[field]		: Returns field associated with LoginID

3	:	[LoginID]?[field]=[update]	: Updates field value associated with LoginID

4	:	[LoginID]?			: Deletes LoginID

5	:	[newLoginID]?[field]=[update]	: Creates new LoginID with unassigned values in unupdated fields if LoginID doesn't exist



# Network Functionality #

If no command is entered, the static client will listen to "localhost:43".

localhost will be able to lookup "location" & update fields of LoginID's.
