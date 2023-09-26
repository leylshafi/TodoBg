using TodoWebService.Data;
using TodoWebService.Services;


namespace TodoWebService.BackgroundServices
{
    public class TodoNotificationService : IHostedService
    {
        private Timer? _timer;
        private readonly IEmailService? _mailService;
        private readonly IServiceProvider _provider;
        public TodoNotificationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        private void Run(object? state)
        {
            using var scope = _provider.CreateScope();
            var _todoDbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            var _mailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var todoitems = _todoDbContext.TodoItems.ToList();
            foreach (var todoitem in todoitems)
            {
                if (todoitem.EndTime.Subtract(DateTime.Today) < TimeSpan.FromDays(1) && !todoitem.AlreadyAlerted)
                {
                    string userEmail = todoitem.Email;
                    string subject = "Overdue To-Do Item";
                    string message = $"Your to-do item '{todoitem.Text}' is overdue.";

                    _mailService.SendEmail(userEmail, subject, message);
                    todoitem.AlreadyAlerted = true;
                    _todoDbContext.Update(todoitem);
                }
            }
            _todoDbContext.SaveChanges();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Background Service started ....");
            _timer = new Timer(Run, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Background Service stopped ....");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }


}

