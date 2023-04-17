﻿namespace EasyKanjiServer.Models.DTOs
{
    public class KanjiDTO
    {
        public int Id { get; set; }
        public string Writing { get; set; } = string.Empty;
        public string OnReadings { get; set; } = string.Empty;
        public string KunReadings { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
    }
}
