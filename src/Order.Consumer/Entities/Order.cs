using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Consumer.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int CodigoPedido { get; set; }
    public int CodigoCliente { get; set; }
    public List<Item> Itens { get; set; }
}