﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class InitDroneResponse : BaseDTO
    {
        [BsonId]
        [BsonElement("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;
    }
}