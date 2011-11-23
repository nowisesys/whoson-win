The calling application should provide an app.config that defines the SOAP service 
connection parameters. This is because the DLL is not loading the app.config by itself, 
instead it relies on the application to provide all settings.

// Anders Lövgren, 2011-11-21
