## Generic EmailNotifier based on Azure Queue trigger 
* Notifier triggers based on Azure Queue message and sends Email.
* Template files are placed in directory named "EmailTemplates".
* Templates are picked based on "TemplateType" property in queue message.
* HandlebarsDotNet is used as the templating lib.
* Basic unit tests added. (Test framework: NUnit)
* test.runsettings file added in JPrime_EmailNotifier.Tests to fetch EnvironmentVariables during Test Runs.

TODO
* Move templates to Blob storage.
* Store secure connections in Azure KeyVault.
* Introduce design patterns/other techniques if "TemplateType" Switch-Case becomes large. 

Queue message sample code
```
 private void AddMessageToEmailQueue(EmailQMessage EmailMessage)
 {
      var storageConnection = Environment.GetEnvironmentVariable("StorageConnection");
      var emailQueueName = Environment.GetEnvironmentVariable("EmailQueueName");
      // Instantiate a QueueClient which will be used to create and manipulate the queue
      QueueClient queueClient = new QueueClient(storageConnection, emailQueueName, new QueueClientOptions
      {
          MessageEncoding = QueueMessageEncoding.Base64
      });

      // Create the queue if it doesn't already exist
      queueClient.CreateIfNotExists();
      if (queueClient.Exists())
          queueClient.SendMessage(JsonSerializer.Serialize(EmailMessage));
  }
  
  public class EmailQMessage
  {
      public string Name { get; set; }
      public string Frequency { get; set; }
      public object EmailConfig { get; set; }
      public string Comments { get; set; }
  }
        
```
