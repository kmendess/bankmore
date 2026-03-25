using BankMore.Application.Models;
using BankMore.Domain.Enums;
using System.Net.Http.Headers;
using Transferencia.API.Infrastructure.Services.DTOs;

namespace Transferencia.API.Infrastructure.Services
{
    public class ContaCorrenteClient
    {
        private readonly HttpClient _httpClient;

        public ContaCorrenteClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<Response> Debitar(string id, int numeroConta, decimal valor)
        {
            var response = await _httpClient.PostAsJsonAsync("conta/movimentar", new
            {
                Id = id,
                NumeroConta = numeroConta,
                Valor = valor,
                Tipo = TipoMovimento.Debito.GetDescription()
            });

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Response>();

                return Response.Error(content!.ErrorType.ToEnum<ErrorType>(), content.Message);
            }

            return Response.Success();
        }

        public async Task<Response> Creditar(string id, int numeroConta, decimal valor)
        {
            var response = await _httpClient.PostAsJsonAsync("conta/movimentar", new
            {
                Id = id,
                NumeroConta = numeroConta,
                Valor = valor,
                Tipo = TipoMovimento.Credito.GetDescription()
            });

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Response>();
                
                return Response.Error(content!.ErrorType.ToEnum<ErrorType>(), content.Message);
            }

            return Response.Success();
        }

        public async Task<ContaCorrenteDTO?> ObterContaLogada()
        {
            var response = await _httpClient.GetAsync("conta/current");

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            Console.WriteLine(response.StatusCode);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ContaCorrenteDTO>();
        }

        public async Task<ContaCorrenteDTO?> ObterContaPorNumero(int numeroConta)
        {
            var response = await _httpClient.GetAsync($"conta/{numeroConta}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ContaCorrenteDTO>();
        }
    }
}
