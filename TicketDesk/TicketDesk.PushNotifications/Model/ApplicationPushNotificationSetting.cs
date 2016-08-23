﻿// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public 
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use. 
//
// For any distribution that contains code from this file, this notice of 
// attribution must remain intact, and a copy of the license must be 
// provided to the recipient.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TicketDesk.PushNotifications.Delivery;
using TicketDesk.Localization;
using TicketDesk.Localization.PushNotifications;

namespace TicketDesk.PushNotifications.Model
{
    [Table("ApplicationPushNotificationSettings", Schema = "notifications")]
    public class ApplicationPushNotificationSetting
    {

        public ApplicationPushNotificationSetting()
        {
            ApplicationName = "TicketDesk";
            IsEnabled = false;
            DeliveryIntervalMinutes = 2;
            AntiNoiseSettings = new AntiNoiseSetting();
            RetryAttempts = 5;
            RetryIntervalMinutes = 2;
            DeliveryProviderSettings = new List<PushNotificationDeliveryProviderSetting> { };
        }

        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(AutoGenerateField = false)]
        public string ApplicationName { get; set; }

        [JsonIgnore]
        [Display(AutoGenerateField = false)]
        [ScaffoldColumn(false)]
        public string Serialized
        {
            get { return JsonConvert.SerializeObject(this); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                var jsettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
                var jData = JsonConvert.DeserializeObject<ApplicationPushNotificationSetting>(value, jsettings);
                IsEnabled = jData.IsEnabled;
                DeliveryIntervalMinutes = jData.DeliveryIntervalMinutes;
                RetryAttempts = jData.RetryAttempts;
                RetryIntervalMinutes = jData.RetryIntervalMinutes;
                DeliveryProviderSettings = jData.DeliveryProviderSettings;
                AntiNoiseSettings = jData.AntiNoiseSettings;
            }
        }

        [NotMapped]
        [Display(Name = "DeliveryProviders", ResourceType = typeof(Strings))]
        public virtual ICollection<PushNotificationDeliveryProviderSetting> DeliveryProviderSettings { get; set; }
            
        [NotMapped]
        [Display(Name = "NotificationsEnabled", Prompt = "NotificationsEnabled_Prompt", ResourceType = typeof(Strings))]
        public bool IsEnabled { get; set; }

        [NotMapped]
        [Display(Name = "DeliveryAttemptInterval", ResourceType = typeof(Strings))]
        [LocalizedDescription("DeliveryAttemptInterval_Description", NameResourceType = typeof(Strings))]
        public int DeliveryIntervalMinutes { get; set; }

        [NotMapped]
        [Display(Name = "NumberOfRetryAttempts", ResourceType = typeof(Strings))]
        [LocalizedDescription("NumberOfRetryAttempts_Description", NameResourceType = typeof(Strings))]
        public int RetryAttempts { get; set; }

        [NotMapped]
        [Display(Name = "InitialRetryInterval", ResourceType = typeof(Strings))]
        [LocalizedDescription("InitialRetryInterval_Description", NameResourceType = typeof(Strings))]
        public int RetryIntervalMinutes { get; set; }

        [NotMapped]
        public AntiNoiseSetting AntiNoiseSettings { get; set; }


        public class PushNotificationDeliveryProviderSetting
        {
            public PushNotificationDeliveryProviderSetting()
            {
                IsEnabled = false;
            }

            [NotMapped]
            public string ProviderAssemblyQualifiedName { get; set; }

            [NotMapped]
            [Display(Name = "Provider", Prompt = "Provider_Prompt", ResourceType = typeof(Strings))]
            public bool IsEnabled { get; set; }

            [NotMapped]
            public JObject ProviderConfigurationData { get; set; }

            public static PushNotificationDeliveryProviderSetting FromProvider(IPushNotificationDeliveryProvider provider)
            {
                return new PushNotificationDeliveryProviderSetting()
                {
                    IsEnabled = false,
                    ProviderAssemblyQualifiedName = provider.GetType().AssemblyQualifiedName,
                    ProviderConfigurationData = JObject.FromObject(provider.Configuration)
                };
            }
        }


        public class AntiNoiseSetting
        {
            public AntiNoiseSetting()
            {
                IsConsolidationEnabled = true;
                InitialConsolidationDelayMinutes = 6;
                MaxConsolidationDelayMinutes = 16;
                ExcludeSubscriberEvents = true;
            }

            [NotMapped]
            [Display(Name = "ConsolidateNotifications", Prompt = "ConsolidateNotifications_Prompt", ResourceType = typeof(Strings))]
            [LocalizedDescription("ConsolidateNotifications_Description", NameResourceType = typeof(Strings))]
            public bool IsConsolidationEnabled { get; set; }

            [NotMapped]
            [Display(Name = "InitialConsolidationDelay", ResourceType = typeof(Strings))]
            [LocalizedDescription("InitialConsolidationDelay_Description", NameResourceType = typeof(Strings))]
            public int InitialConsolidationDelayMinutes { get; set; }

            [NotMapped]
            [Display(Name = "MaximumConsolidationDelay", ResourceType = typeof(Strings))]
            [LocalizedDescription("MaximumConsolidationDelay_Description", NameResourceType = typeof(Strings))]
            public int MaxConsolidationDelayMinutes { get; set; }

            [NotMapped]
            [Display(Name = "ExcludeSubscribersOwnEvents", Prompt = "ExcludeSubscribersOwnEvents_Prompt", ResourceType = typeof(Strings))]
            [LocalizedDescription("ExcludeSubscribersOwnEvents_Description", NameResourceType = typeof(Strings))]
            public bool ExcludeSubscriberEvents { get; set; }
        }
    }
}
