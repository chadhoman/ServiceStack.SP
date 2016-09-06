# ServiceStack.SP
SharePoint Specific Implementation of ServiceStack

This uses the 4.0.0.0 version of ServiceStack. 

The solution is built for a SharePoint 2013 farm solution. The custom services are hosted at _layouts/api.


#Setup
There is one web app level feature that needs to be activated.  This feature adds the correct web.config entries for ServiceStack to work. 

Instead of modifying the global.asax for your SharePoint site (which cannot be done in a WSP package), there is a custom HTTPModule that will run once to start your ServiceStack application. 

#Usage
Deploy the solution to your farm.  Navigate to http://sitecoll/_layouts/api/metadata and you will see the metadate about the service.  You can add other services as you normally would in ServiceStack. 
Swagger UI is included to test the service at http://sitecoll/_layouts/api/swagger-ui
 