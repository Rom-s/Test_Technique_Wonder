using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MaterialMaker : MonoBehaviour
{
    [SerializeField] string baseMapUrl;
    [SerializeField] string emissiveUrl;
    [SerializeField] string metallicRoughnessUrl;
    [SerializeField] string normalUrl;
    [SerializeField] string occlusionUrl;
    [SerializeField] Slider slider;

    private Material _material;

    // Start is called before the first frame update
    void Start()
    {
        _material = new Material(GetComponent<Renderer>().material);
        StartCoroutine(CreateMaterial());
    }

    IEnumerator CreateMaterial()
    {
        _material.EnableKeyword("_METALLICGLOSSMAP");
        _material.EnableKeyword("_EMISSION");
        _material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        _material.SetColor("_EmissionColor", Color.white);

        yield return StartCoroutine(GetRequest(baseMapUrl, "_MainTex"));
        yield return StartCoroutine(GetRequest(emissiveUrl, "_EmissionMap"));
        yield return StartCoroutine(GetRequest(metallicRoughnessUrl, "_MetallicGlossMap"));
        yield return StartCoroutine(GetRequest(normalUrl, "_BumpMap"));
        yield return StartCoroutine(GetRequest(occlusionUrl, "_OcclusionMap"));

        yield return new WaitForSeconds(0.5f);
        slider.gameObject.SetActive(false);
        GetComponent<Renderer>().material = _material;
    }

    IEnumerator GetRequest(string uri, string textureName)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
        {
            slider.value += 1f / 10f;

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;

                    // Particular case of the metallic map
                    if (textureName == "_MetallicGlossMap")
                    {
                        // Put G channel to R and B to A
                        Color[] pixels = texture.GetPixels();
                        Color[] resultingPixels = new Color[pixels.Length];
                        for (int c = 0; c < pixels.Length; c++)
                        {
                            resultingPixels[c] = new Color(pixels[c].g, pixels[c].a, pixels[c].b, pixels[c].b);
                        }
                        texture.SetPixels(resultingPixels);
                        texture.Apply();
                    }

                    _material.SetTexture(textureName, texture);
                    slider.value += 1f / 10f;
                    break;
            }
        }
    }
}
