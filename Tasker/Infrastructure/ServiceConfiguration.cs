﻿namespace Tasker.Infrastructure
{
    public class ServiceConfiguration
    {
        public string LoggingUrl { get; set; }
        public string EventBusQueueName { get; set; }
        public RabbitMqConfiguration EventBus { get; set; }
    }

    public class PortfolioRepositoryOptions
    {
        public string MongoDbUrl { get; set; }
        public string Database { get; set; }
        public string UserCollectionName { get; set; }
        public string TransactionCollectionName { get; set; }
        public string CronJobCollectionName { get; set; }
    }

    public class JwtAuthorizationConfigurationOptions
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
    }

    public class DirectoryPathConfigurationOptions
    {
        public string ProfilePicture { get; set; }
        public string Download { get; set; }
    }

    public class BaseAddressConfigurationOptions
    {
        public string GoogleDrive { get; set; }
        public string GoogleSpreadsheet { get; set; }
        public string OneDrive { get; set; }
    }

    public class RabbitMqConfiguration
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RetryCount { get; set; }
        public string ClientProvidedName { get; set; }
    }

    public class GoogleAuthServiceConfigurationOptions
    {
        public string ServiceUrl { get; set; }
    }

    public class MicrosoftGraphServiceConfigurationOptions
    {
        public string ServiceUrl { get; set; }
    }

    public class MicrosoftAuthServiceConfigurationOptions
    {
        public string ServiceUrl { get; set; }
    }

    public class CronJobConfigurationOptions
    {
        public string SyncBalanceCronSchedule { get; set; }
    }
}