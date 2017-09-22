﻿namespace Atom.Server.Clients
{
    public class ClientInformations
    {
        // Fields
        private Client _client;


        // Constructor
        public ClientInformations(Client client)
        {
            _client = client;
            SetDefault();
        }


        // Properties
        public int Id { get; set; }

        public string Name { get; set; }

        public void SetDefault()
        {
            Id = -1;
            Name = "";
        }

        public override string ToString()
        {
            return $"({Id}:{Name})";
        }
    }
}