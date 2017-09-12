
This is the client side application for the whoson web service. It is part of 
an application suite that together forms an centralized logon accounting service.

See http://it.bmc.uu.se/andlov/proj/whoson for more information.

** SERVER SIDE:

   You need to install either whoson-asp or whoson-php on an web server to use 
   these applications.

** CONFIGURATION:

   The application settings are configured in whoson.exe.config. The default 
   configuration is setup to use a WhosOn SOAP service on localhost. At least 
   change the endpoint URL.

** BUFFER SIZES:

   The default size of buffers are rather small, you might need to adjust these
   to get listings work.  These are set in whoson.exe.config (app.config in source 
   code), increase the value of maxReceivedMessageSize and maxBufferSize as needed.

** EXAMPLES:

   o) Get 20 records starting at ID 1000:

      whoson.exe -l --id=1000 --after -L 20

   o) Close session 1005 - 1010:
     
      whoson.exe -l --first=1005 --last=1010 -F

   o) Close all active logon sessions:

      whoson.exe -l -a -F

** SERVICE:

   It's also possible to run whoson as a Windows service:

   o) Register service:

      whosond.exe -i -U username -P secret	// HTTP basic auth
      whosond.exe -i -W				// Windows builtin logon

   The service is configured in whosond.exe.config similar to whoson.exe, the
   standalone client.
       

// Anders Lövgren, 2011-10-17
