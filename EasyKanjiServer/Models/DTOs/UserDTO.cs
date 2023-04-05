﻿namespace EasyKanjiServer.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<Kanji> Kanjis { get; set; } = new();
    }
}
