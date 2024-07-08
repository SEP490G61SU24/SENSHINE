﻿using API.Models;

namespace API.Services
{
    public interface IBedService
    {
        Task<Bed> AddBed(string bedNumber);
        Task<Bed> UpdateBed(int id, string bedNumber);
        Task<bool> DeleteBed(int id);
        Task<Bed> GetBedById(int id);
        Task<IEnumerable<Bed>> GetAllBeds();
    }
}