﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avioane
{
    internal class Player
    {        
        public Grid PlayerGrid { get; private set; }
        public List<Airplane> Airplanes { get; private set; }

        public Player()
        {
            PlayerGrid = new Grid();
            Airplanes = new List<Airplane>();
        }

        public bool AddAirplane(Airplane airplane)
        {
            if (PlayerGrid.CanPlaceAirplane(airplane))
            {
                PlayerGrid.PlaceAirplane(airplane);
                Airplanes.Add(airplane);
                return true;
            }
            return false;
        }
    }
}