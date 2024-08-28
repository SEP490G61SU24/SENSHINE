using API.Services;
using Quartz;

namespace API.CronJobs
{
    public class UpdateWorkScheduleJob : IJob
    {
        private readonly IWorkScheduleService _workScheduleService;

        public UpdateWorkScheduleJob(IWorkScheduleService workScheduleService)
        {
            _workScheduleService = workScheduleService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _workScheduleService.CreateWorkSchedulesForNextTwoMonths();
                await _workScheduleService.UpdateWorkSchedulesStatus();
			}
            catch (Exception ex)
            {
                Console.WriteLine($"CRON LỖI: {ex.Message}");
            }
        }
    }
}
