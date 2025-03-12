﻿namespace Cinema.Web.Models
{
    public class ScreeningViewModel
    {
        /// <summary>
        /// Unique identifier for the screening
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets or Sets Movie
        /// </summary>
        public required MovieViewModel Movie { get; init; }

        /// <summary>
        /// Gets or Sets Room
        /// </summary>
        public required RoomViewModel Room { get; init; }

        /// <summary>
        /// Start time of the screening
        /// </summary>
        public DateTime StartsAt { get; init; }

        /// <summary>
        /// Price of the screening
        /// </summary>
        public decimal Price { get; init; }
    }
}
