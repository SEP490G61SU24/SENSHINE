﻿using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface IWorkScheduleService
    {
        ////Task<WorkScheduleDTO> AddWorkSchedule(WorkScheduleDTO workScheduleDto);
        //Task<WorkScheduleDTO> UpdateWorkSchedule(int id, WorkScheduleDTO workScheduleDto);
        //Task<bool> DeleteWorkSchedule(int id);
        //Task<WorkScheduleDTO> GetWorkScheduleById(int id);
        //Task<IEnumerable<WorkScheduleDTO>> GetAllWorkSchedules();
        Task<PaginatedList<UserSlotDTO>> GetWorkSchedules(int pageIndex, int pageSize, string searchTerm, string spaId);
        //Task<IEnumerable<WorkScheduleDTO>> GetWorkSchedulesByWeek(int employeeId, DateTime startDate, DateTime endDate);
        //Task<IEnumerable<WeekOptionDTO>> GetAvailableWeeks(int employeeId, int year);
        //Task<IEnumerable<int>> GetAvailableYears(int employeeId);
        //Task CreateWorkSchedulesForNextTwoMonths();
        Task UpdateWorkSchedulesStatus(int id);
        Task<string> GetStatusEmployeeInThisSlot(int employeeId, int slotId, DateTime date);
        //Task UpdateWorkScheduleStatusForAppointment(int empId, DateTime appointmentDate, string slot, string newStatus);
        Task<UserSlot> AddWorkThisEmployee(int employeeId, int slotId, DateTime date);
    }
}
