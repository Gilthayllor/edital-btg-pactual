﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ordering.Consumer.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public int CodigoPedido { get; set; }
    public int CodigoCliente { get; set; }
    public List<Item> Items { get; set; } = [];
}