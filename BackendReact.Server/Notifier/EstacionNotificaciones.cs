using BackendReact.Server.HUB;
using Microsoft.AspNetCore.SignalR;
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace BackendReact.Server.Notifier
{
    public class EstacionNotificaciones
    {
        private readonly IHubContext<EstacionHUB> _hubContext;
        private readonly string _connectionString;

        public EstacionNotificaciones(IHubContext<EstacionHUB> hubContext, string connectionString)
        {
            _hubContext = hubContext;
            _connectionString = connectionString;
            SuscribirseACambios();
        }

        public void SuscribirseACambios()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT ID_ESTACION, ESTATUS FROM andon.CAT_ESTACION", connection))
                {
                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += OnDataChange;

                    // Execute the command to initiate the dependency
                    command.ExecuteNonQuery();
                }
            }
        }

        private void OnDataChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                var cambios = ObtenerCambiosEstatus();

                foreach (var cambio in cambios)
                {
                    _hubContext.Clients.All.SendAsync("RecibirCambioEstatus", cambio.IdEstacion, cambio.Estatus);
                }

                // Resubscribe for continuous notifications
                SuscribirseACambios();
            }
        }

        private List<(int IdEstacion, int Estatus)> ObtenerCambiosEstatus()
        {
            var cambios = new List<(int IdEstacion, int Estatus)>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(
                    "SELECT ID_ESTACION, ESTATUS FROM andon.CAT_ESTACION", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idEstacion = reader.GetInt32(0);
                            int estatus = reader.GetInt32(1);
                            cambios.Add((idEstacion, estatus));
                        }
                    }
                }
            }
            return cambios;
        }
    }
}