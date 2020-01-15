﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace IntegracaoBancos
{
    class MapeadorDadosSql
    {
        public List<RegistroEntrada> BuscaRegistroDoDia(int ultimoAluno)
        {
            var registroEntradas = new List<RegistroEntrada>();
            using (SqlConnection sqlConn = SqlConecao())
            {
                sqlConn.Open();
                var cmd = new SqlCommand("SELECT [CD_LOG_ACESSO] ,[NU_MATRICULA] ,[NU_DATA_REQUISICAO] ,[TP_SENTIDO_CONSULTA] FROM [dbo].[LOG_ACESSO] " +
                                          $"WHERE [CD_LOG_ACESSO] > {ultimoAluno} ORDER by [CD_LOG_ACESSO]", sqlConn);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        registroEntradas.Add(new RegistroEntrada
                        {
                            Id = dr.GetInt32(0),
                            Matricula = dr.GetInt32(1),
                            Horario = dr.GetDateTime(2),
                            Sentido = dr.GetInt32(3)
                        });
                    }
                }

                sqlConn.Close();
            }
            return registroEntradas;
        }

        private SqlConnection SqlConecao()
        {
            var configuracao = new LeituraConfiguração().lerConfiguracao();
            string stringdeConecao = $"server={configuracao.nomeServidor};database={configuracao.nomeBanco};Integrated Security=SSPI;User Id={configuracao.usuario};Password={configuracao.senha};";
            string connectionString = stringdeConecao;
            return new SqlConnection(connectionString);
        }

        public void InserirRegistros(RegistroEntrada registroEntrada)
        {
            try
            {
                var sqlConn = SqlConecao();
                sqlConn.Open();
                string sql = $"INSERT INTO [dbo].[LOG_ACESSO]([CD_LOG_ACESSO], [NU_MATRICULA], [NU_DATA_REQUISICAO], [TP_SENTIDO_CONSULTA]) VALUES({registroEntrada.Id}, {registroEntrada.Matricula}, '{registroEntrada.Horario}', {registroEntrada.Sentido})";
                var cmd = new SqlCommand(sql, sqlConn);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            catch (Exception)
            {
                
                throw;
            }


        }
        public RegistroEntrada BuscaUltimoRegistro()
        {
            var registroEntradas = new List<RegistroEntrada>();
            using (SqlConnection sqlConn = SqlConecao())
            {
                sqlConn.Open();
                var cmd = new SqlCommand("SELECT [CD_LOG_ACESSO] ,[NU_MATRICULA] ,[NU_DATA_REQUISICAO] ,[TP_SENTIDO_CONSULTA] FROM [dbo].[LOG_ACESSO] ORDER by [CD_LOG_ACESSO]", sqlConn);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        registroEntradas.Add(new RegistroEntrada
                        {
                            Id = dr.GetInt32(0),
                            Matricula = dr.GetInt32(1),
                            Horario = dr.GetDateTime(2),
                            Sentido = dr.GetInt32(3)
                        });
                    }
                }

                sqlConn.Close();
            }
            var registro = registroEntradas[registroEntradas.Count - 1];
            return registro;
        }
    }
}