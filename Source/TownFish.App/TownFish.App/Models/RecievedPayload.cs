using System;
using Realms;

namespace TownFish.App.Models
{
    public class RecievedPayload : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Content { get; set; }
    }
}
