
using TodoWebService.Data;
using TodoWebService.Models.Entities;
using TodoWebService.Services;

namespace TodoWebService.BackgroundServices
{
    public class TodoNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public TodoNotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                    var allTodos = context.TodoItems.ToList();

                    var currentTime = DateTime.Now;
                    var overdueTodos = allTodos.Where(todo => todo.ScheduledMinutes < currentTime.Minute - todo.CreatedTime.Minute).ToList();

                    foreach (var todo in overdueTodos)
                    {
                        string userEmail = todo.Email; 
                        string subject = "Overdue To-Do Item";
                        string message = $"Your to-do item '{todo.Text}' is overdue.";

                        await emailService.SendEmailAsync(userEmail, subject, message);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Check every 10 minutes
            }
        }

        
    }
}
