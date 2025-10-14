using CAASD.Core.Interfaces;
using CAASD.Integracion.Data;
using CAASD.Integracion.Services;
using CAASD.Integracion.UnitOfWork;

namespace CAASD.Caja1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            CAASDContext context = new CAASDContext();

            IUnitOfWork unitOfWork = new UnitOfWork(context);

            AuthService authService = new AuthService(unitOfWork);

            Application.Run(new LoginForm());
        }
    }
}
