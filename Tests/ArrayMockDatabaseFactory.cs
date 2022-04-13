// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Dynamic.Core;
// using System.Threading.Tasks;
// using Domain.Entities;
// using MongoDB.Driver;
// using Moq;
// using Moq.Language.Flow;
// using SimDrone;

namespace Tests;

public class ArrayMockDatabaseFactory
{
//     
//     private Mock<IMongoCollection<Order>> _orderCollection;
//     private Mock<IMongoCollection<DroneRecord>> _droneCollection;
//     private Mock<IAsyncCursor<DroneRecord>> _droneCursor;
//     private Mock<IAsyncCursor<Order>> _orderCursor;
//
//     public ArrayMockDatabaseFactory(DroneRecord[] drones, Order[] orders)
//     {
//         _droneCollection = new Mock<IMongoCollection<DroneRecord>>();
//         _orderCollection = new Mock<IMongoCollection<Order>>();
//         _droneCursor = new Mock<IAsyncCursor<DroneRecord>>();
//         _orderCursor = new Mock<IAsyncCursor<Order>>();
//         // I honestly don't know how sequences work in moq, but was suggested to use this
//         _droneCursor.SetupSequence(x => x.MoveNext(default)).Returns(true).Returns(false);
//         _orderCursor.SetupSequence(x => x.MoveNext(default)).Returns(true).Returns(false);
//         _orderCursor.SetupGet(x => x.Current).Returns(orders.ToList);
//         _droneCursor.SetupGet(x => x.Current).Returns(drones.ToList);
//         
//     }
//
//     public void NewArrayMockDatabase(DroneRecord[] drones, Order[] orders)
//     {
//         _droneCollection = new Mock<IMongoCollection<DroneRecord>>();
//         _orderCollection = new Mock<IMongoCollection<Order>>();
//         // I honestly don't know how sequences work in moq, but was suggested to use this
//         _droneCursor = new Mock<IAsyncCursor<DroneRecord>>();
//         _orderCursor = new Mock<IAsyncCursor<Order>>();
//         _droneCursor.SetupSequence(x => x.MoveNext(default)).Returns(true).Returns(false);
//         _orderCursor.SetupSequence(x => x.MoveNext(default)).Returns(true).Returns(false);
//         _orderCursor.SetupGet(x => x.Current).Returns(orders.ToList);
//         _droneCursor.SetupGet(x => x.Current).Returns(drones.ToList);
//     }
//
//     public IMongoCollection<DroneRecord> getDroneDatabase()
//     {
//         setupDroneDatabase();
//         return _droneCollection.Object;
//     }
//     public IMongoCollection<Order> getOrderDatabase()
//     {
//         setupOrderDatabase();
//         return _orderCollection.Object;
//     }
//
//     // Nasty solution from Pablo Carmona's solution to a stack overflow problem
//     private void setupOrderDatabase()
//     {
//         _orderCollection.Setup(x => x.FindAsync<Order>(Builders<Order>.Filter.Empty,It.IsAny<FindOptions<Order>>(), default)).Returns(Task.FromResult(_orderCursor.Object));
//         _orderCollection.Setup(x => x.FindSync<Order>(Builders<Order>.Filter.Empty,It.IsAny<FindOptions<Order>>(), default)).Returns(_orderCursor.Object);
//         _orderCollection.Setup(x => x.Find<Order>(Builders<Order>.Filter.Empty, default)).Returns(_orderCursor.Object);
//
//     }
//
//     private void setupDroneDatabase()
//     {
//         _droneCollection.Setup(x => x.FindAsync<DroneRecord>(Builders<DroneRecord>.Filter.Empty,It.IsAny<FindOptions<DroneRecord>>(), default)).Returns(Task.FromResult(_droneCursor.Object));
//         _droneCollection.Setup(x => x.FindSync<DroneRecord>(Builders<DroneRecord>.Filter.Empty,It.IsAny<FindOptions<DroneRecord>>(), default)).Returns(_droneCursor.Object);
//
//     }
//
}