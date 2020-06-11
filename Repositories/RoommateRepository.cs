using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Roommate data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class RoommateRepository : BaseRepository
    {
        /// <summary>
        ///  When new RoomRespository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public RoommateRepository(string connectionString) : base(connectionString) { }

        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {           
                conn.Open();
                
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    
                    cmd.CommandText = "SELECT Id, Firstname, Lastname, RentPortion, MoveInDate, RoomId FROM Roommate";

                    
                    SqlDataReader reader = cmd.ExecuteReader();

                    
                    List<Roommate> roommates = new List<Roommate>();

                    
                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        int FirstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string FirstNameValue = reader.GetString(FirstNameColumnPosition);

                        int LastNameColumnPosition = reader.GetOrdinal("LastName");
                        string LastNameValue = reader.GetString(LastNameColumnPosition);

                        int RentPortionColumnPosition = reader.GetOrdinal("RentPortion");
                        int RentPortionValue = reader.GetInt32(RentPortionColumnPosition);

                        int MovedInDateColumnPosition = reader.GetOrdinal("MoveInDate");
                        DateTime MovedInDateValue = reader.GetDateTime(MovedInDateColumnPosition);

                        

                        // Now let's create a new roommate object using the data from the database.
                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            Firstname = FirstNameValue,
                            Lastname = LastNameValue,
                            RentPortion = RentPortionValue,
                            MovedInDate = MovedInDateValue,
                            Room = null
                        };

                        // ...and add that roommate object to our list.
                        roommates.Add(roommate);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of roommates to whomever called this method.
                    return roommates;
                }
            }
        }

        /// <summary>
        ///  Returns a single roommate with the given id.
        /// </summary>
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Firstname, Lastname, RentPortion, MoveInDate, RoomId FROM Roommate";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        roommate = new Roommate
                        {
                            Id = id,
                            Firstname = reader.GetString(reader.GetOrdinal("Firstname")),
                            Lastname = reader.GetString(reader.GetOrdinal("Lastname")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                            Room = new Room()
                        };
                    }

                    reader.Close();

                    return roommate;
                }
            }
        }

        public List<Roommate> GetAllWithRoom(int roomId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT rm.Id, rm.Firstname, rm.Lastname, rm.RentPortion, rm.MoveInDate, 
                                        rm.RoomId, r.Name, r.MaxOccupancy 
                                        FROM Roommate rm join room r on rm.RoomId = r.id
                                        where roomId = @roomId";
                    cmd.Parameters.AddWithValue("@roomId", roomId);
                                        

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Roommate> roommates = new List<Roommate>();


                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        int FirstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string FirstNameValue = reader.GetString(FirstNameColumnPosition);

                        int LastNameColumnPosition = reader.GetOrdinal("LastName");
                        string LastNameValue = reader.GetString(LastNameColumnPosition);

                        int RentPortionColumnPosition = reader.GetOrdinal("RentPortion");
                        int RentPortionValue = reader.GetInt32(RentPortionColumnPosition);

                        int MovedInDateColumnPosition = reader.GetOrdinal("MoveInDate");
                        DateTime MovedInDateValue = reader.GetDateTime(MovedInDateColumnPosition);

                        int RoomId = reader.GetInt32(reader.GetOrdinal("RoomId"));

                        string roomName = reader.GetString(reader.GetOrdinal("name"));

                        int maxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"));

                        // Now let's create a new roommate object using the data from the database.
                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            Firstname = FirstNameValue,
                            Lastname = LastNameValue,
                            RentPortion = RentPortionValue,
                            MovedInDate = MovedInDateValue,
                            Room = new Room()
                            {
                                Id = RoomId,
                                Name = roomName,
                                MaxOccupancy = maxOccupancy
                            }
                        };

                        // ...and add that roommate object to our list.
                        roommates.Add(roommate);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of roommates to whomever called this method.
                    return roommates;
                }
            }
        }





        public void Insert(Roommate roommate)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // These SQL parameters are annoying. Why can't we use string interpolation?
                    // ... SQL injection attacks!!!
                    cmd.CommandText = @"INSERT INTO Roommate (FirstName, LastName, RentPortion, MoveInDate, RoomId) 
                                         OUTPUT INSERTED.Id 
                                         VALUES (@FirstName, @LastName, @RentPortion, @MoveInDate, @RoomID)";
                    cmd.Parameters.AddWithValue("@Firstname", roommate.Firstname);
                    cmd.Parameters.AddWithValue("@Lastname", roommate.Lastname);
                    cmd.Parameters.AddWithValue("@RentPortion", roommate.RentPortion);
                    cmd.Parameters.AddWithValue("@MoveInDate", roommate.MovedInDate);
                    cmd.Parameters.AddWithValue("@RoomID", roommate.Room.Id);

                    int roommateId = (int)cmd.ExecuteScalar();

                    roommate.Id = roommateId;
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }
        public void Update(Room room)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Room
                                    SET Name = @name,
                                        MaxOccupancy = @maxOccupancy
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", room.Name);
                    cmd.Parameters.AddWithValue("@maxOccupancy", room.MaxOccupancy);
                    cmd.Parameters.AddWithValue("@id", room.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Room WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
