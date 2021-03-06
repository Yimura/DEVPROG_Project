﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EindProject.Models
{
    public class Stats
    {
        [JsonProperty(PropertyName = "categories")]
        public List<Category> Categories { get; set; }

        [JsonProperty(PropertyName = "editors")]
        public List<Editor> Editors { get; set; }

        [JsonProperty(PropertyName = "languages")]
        public List<Language> Languages { get; set; }

        [JsonProperty(PropertyName = "projects")]
        public List<Project> Projects { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public Guid Guid { get; set; }

        [JsonProperty(PropertyName = "human_readable_daily_average")]
        public string HumanReadableAverageDaily { get; set; }

        [JsonProperty(PropertyName = "human_readable_total")]
        public string HumanReadableTotal { get; set; }

        public string Photo 
        {
            get
            {
                return "https://wakatime.com/gravatar/" + this.Guid;
            }        
        }
    }
}
