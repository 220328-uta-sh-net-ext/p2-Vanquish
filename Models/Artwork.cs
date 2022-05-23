﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    internal class Artwork
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ArtWorkName { get; set; } = string.Empty;
        public Byte[]? ArtWorkData { get; set; }

    }
}