﻿using System;
using System.Collections.Generic;
using System.Linq;
using FriendStorage.Model;
using System.IO;
using Newtonsoft.Json;

namespace FriendStorage.DataAccess
{
    public class FileDataService : IDataService
    {
        private const string _storageFile = "Friends.json";

        public virtual Friend GetFriendById(int friendId)
        {
            var friends = ReadFromFile();
            return friends.Single(f => f.Id == friendId);
        }

        public virtual void SaveFriend(Friend friend)
        {
            if (friend.Id <= 0)
            {
                InsertFriend(friend);
            }
            else
            {
                UpdateFriend(friend);
            }
        }

        public virtual void DeleteFriend(int friendId)
        {
            var friends = ReadFromFile();
            var existing = friends.Single(f => f.Id == friendId);
            friends.Remove(existing);
            SaveToFile(friends);
        }

        private void UpdateFriend(Friend friend)
        {
            var friends = ReadFromFile();
            var existing = friends.Single(f => f.Id == friend.Id);
            var indexOfExisting = friends.IndexOf(existing);
            friends.Insert(indexOfExisting, friend);
            friends.Remove(existing);
            SaveToFile(friends);
        }

        private void InsertFriend(Friend friend)
        {
            var friends = ReadFromFile();
            var maxFriendId = friends.Count == 0 ? 0 : friends.Max(f => f.Id);
            friend.Id = maxFriendId + 1;
            friends.Add(friend);
            SaveToFile(friends);
        }

        public virtual IEnumerable<LookupItem> GetAllFriends()
        {
            return ReadFromFile()
            .Select(f => new LookupItem
            {
                Id = f.Id,
                DisplayMember = $"{f.FirstName} {f.LastName}"
            });
        }

        public void Dispose()
        {
            // Usually Service-Proxies are disposable. This method is added as demo-purpose
            // to show how to use an IDisposable in the client with a Func<T>. =>  Look for example at the FriendDataProvider-class
        }

        private void SaveToFile(List<Friend> friendList)
        {
            var json = JsonConvert.SerializeObject(friendList, Formatting.Indented);
            File.WriteAllText(_storageFile, json);
        }

        private List<Friend> ReadFromFile()
        {
            if (!File.Exists(_storageFile))
            {
                return new List<Friend>
                {
                    new Friend{Id=1,FirstName = "Thomas",LastName="Huber",
                        Birthday = new DateTime(1980,10,28), IsDeveloper = true},
                    new Friend{Id=2,FirstName = "Julia",LastName="Huber",
                        Birthday = new DateTime(1982,10,10)},
                    new Friend{Id=3,FirstName="Anna",LastName="Huber",
                        Birthday = new DateTime(2011,05,13)},
                    new Friend{Id=4,FirstName="Sara",LastName="Huber",
                        Birthday = new DateTime(2013,02,25)},
                    new Friend{Id=5,FirstName = "Andreas",LastName="Böhler",
                        Birthday = new DateTime(1981,01,10), IsDeveloper = true},
                    new Friend{Id=6,FirstName="Urs",LastName="Meier",
                        Birthday = new DateTime(1970,03,5), IsDeveloper = true},
                     new Friend{Id=7,FirstName="Chrissi",LastName="Heuberger",
                        Birthday = new DateTime(1987,07,16)},
                     new Friend{Id=8,FirstName="Erkan",LastName="Egin",
                        Birthday = new DateTime(1983,05,23)},
                };
            }

            var json = File.ReadAllText(_storageFile);
            return JsonConvert.DeserializeObject<List<Friend>>(json);
        }
    }
}