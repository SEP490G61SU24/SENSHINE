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
            await _workScheduleService.GetAllWorkSchedules();
        }
    }
}
