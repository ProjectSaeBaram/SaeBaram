using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class HTTPSystem
{
    private readonly HttpClient _client;
    private readonly string _apiURL;
    private readonly TextCutScene _textCutScene;

    // todo TextCutScene 객체 넣고, _apiURL 값 적절히 수정하기

    public HTTPSystem(TextCutScene _textCutScene)
    {
        _client = new HttpClient();
        _apiURL = "http://203.255.57.136:5000/" + _textCutScene.SceneID;
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetSheetDataAsync()
    {
        HttpResponseMessage response = await _client.GetAsync(_apiURL);
        response.EnsureSuccessStatusCode();

        // UTF-8 인코딩으로 응답 본문을 읽음
        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
        string responseBody = Encoding.UTF8.GetString(responseBytes);

        return responseBody;
    }
}
