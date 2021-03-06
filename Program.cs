﻿using System;
using System.Collections.Generic;
using System.Linq;
using Roommates.Models;
using Roommates.Repositories;

namespace Roommates
{
    class Program
    {
        /// <summary>
        ///  This is the address of the database.
        ///  We define it here as a constant since it will never change.
        /// </summary>
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true";

        static void Main(string[] args)
        {
            
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);

            Console.WriteLine("Getting All Rooms:");
            Console.WriteLine();

            List<Room> allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }
            Console.WriteLine("----------------------------");
            Console.WriteLine("Getting Room with Id 1");

            Room singleRoom = roomRepo.GetById(1);

            Console.WriteLine($"{singleRoom.Id} {singleRoom.Name} {singleRoom.MaxOccupancy}");

            Room bathroom = new Room
            {
                Name = "Bathroom",
                MaxOccupancy = 1
            };

            // roomRepo.Insert(bathroom);

            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Added the new Room with id {bathroom.Id}");
            

            /*
        Room updatedBathroom = new Room
        {
            Name = "Linen Closet",
            MaxOccupancy = 1,
            Id = bathroom.Id
        };
        roomRepo.Update(updatedBathroom);

            allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }
            */




            /*
        roomRepo.Delete(17);

            allRooms = roomRepo.GetAll();

            foreach (Room room in allRooms)
            {
                Console.WriteLine($"{room.Id} {room.Name} {room.MaxOccupancy}");
            }
            */
            RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);
            List<Roommate> allRoommates = roommateRepo.GetAll();

            foreach (Roommate roommate in allRoommates)
            {
                Console.WriteLine($"{roommate.Firstname} {roommate.Lastname} {roommate.RentPortion} {roommate.MovedInDate}");
            }
            /*
            Room singleRoommate = allRoommates.GetById(1);

            Console.WriteLine($"{singleRoommate.Firstname} {singleRoommate.Lastname} {singleRoommate.room.Name}");
            */
            List<Room> someRooms = roomRepo.GetAll();
            Room aRoom = someRooms.First();

            Roommate newRoommate = new Roommate()
            {
                Firstname = "Carrie",
                Lastname = "Wilson",
                RentPortion = 76,
                MovedInDate = DateTime.Now.AddDays(-1),
                Room = aRoom
            };

            // roommateRepo.Insert(newRoommate);

            List<Roommate> roommates = roommateRepo.GetAllWithRoom(3);
        }

}
}