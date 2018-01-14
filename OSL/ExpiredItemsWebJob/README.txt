ExpiredItemsWebJob is a scheduled WebJob added to the AppService that when triggered updates all of the expired donations with status Listed to have status Wasted. What is currently published is in WasteExpiredListings.zip and scheduled to run every day at 11:59pm.

The contents of WasteExpiredListings.zip are the published output of the AzureWebJob solution and the run.sh script. The solution was published using 'dotnet -c Release -r win-x86'. The run.sh script starts the application with 'dotnet AzureWebJob.dll'.

To change the web job, make your changes to the solution, publish and zip with the run script as described above, then go to the Azure portal. In the Azure portal go to the AppService, scroll down to Settings, click on WebJobs, then click the add button at the top. Use the zip file where it says file upload, for type choose Triggered and Scheduled and then give a schedule. Currently it is 0 59 23 * * *, which is at 0 seconds, 59 minutes and 23 hours of every day.

The AzureWebJob solution will need to be updated if changes are made to the DonationStatus model or to the database location or name.
