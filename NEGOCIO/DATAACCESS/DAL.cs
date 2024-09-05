using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NEGOCIO.MODELO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace NEGOCIO.DATAACCESS
{
    public class DAL
    {
        private readonly string _connectionString;

        public DAL(IOptions<ConnectionStrings> options)
        {
            _connectionString = options.Value.DB_SQL;
        }

        public bool SeleccionaClienteTenedor(ref string sError, List<cEstacion> lstCliente)
        {
            try
            {
                using SqlConnection connection = new(_connectionString);
                using SqlCommand command = new("andon.SEL_ESTACION", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                using SqlDataReader oResultado = command.ExecuteReader();
                while (oResultado.Read())
                {
                    if (Convert.ToInt32(oResultado["ExisteError"]) == 0)
                    {
                        cEstacion oEstacion = new cEstacion();
                        oEstacion.ID_ESTACION = Convert.ToInt32(oResultado["ID_ESTACION"]);
                        oEstacion.D_ESTACION = Convert.ToString(oResultado["D_ESTACION"]);
                        oEstacion.ESTATUS = Convert.ToString(oResultado["ESTATUS"]);
                        lstCliente.Add(oEstacion);
                    }
                    else if (Convert.ToInt32(oResultado["ExisteError"]) == 1)
                    {
                        sError = "Ocurrió un problema al procesar la solicitud, contacte al administrador del sistema con el siguiente folio: " +
                            GeneraLog.LogError(oResultado["MensajeError"].ToString() ?? string.Empty, "SQL");
                        return false;
                    }
                    else if (Convert.ToInt32(oResultado["ExisteError"]) == 2)
                    {
                        sError = oResultado["MensajeError"]?.ToString() ?? string.Empty;
                        return false;
                    }
                    else throw new ArgumentException("La base de datos no regreso ningun valor en ExisteError verificar la consulta");
                }
                if (oResultado.NextResult())
                {
                    if (oResultado.Read())
                    {
                        if (Convert.ToInt32(oResultado["ExisteError"]) == 1)
                        {
                            sError = "Ocurrió un problema al procesar la solicitud, contacte al administrador del sistema con el siguiente folio: " +
                                GeneraLog.LogError(oResultado["MensajeError"].ToString() ?? string.Empty, "SQL");
                            return false;
                        }
                        else throw new ArgumentException("La base de datos no regreso ningun valor en ExisteError verificar la consulta");
                    }
                    else throw new ArgumentException("La base de datos no regreso ningun valor en ExisteError verificar la consulta");
                }
                return true;
            }
            catch (Exception ex)
            {
                sError = "Ocurrió un problema al procesar la solicitud, contacte al administrador del sistema con el siguiente folio: " + GeneraLog.LogError(ex.ToString(), "DAL");
                return false;
            }
        }
    }
}
