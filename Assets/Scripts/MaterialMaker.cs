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
        slider.value = 0;
        _material = new Material(GetComponent<Renderer>().material);
        StartCoroutine(CreateMaterial());
    }

    /// <summary>
    /// Create the material using several web requests
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateMaterial()
    {
        // Enable some features of the shader
        _material.EnableKeyword("_METALLICGLOSSMAP");
        _material.EnableKeyword("_EMISSION");
        _material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        _material.SetColor("_EmissionColor", Color.white);

        // Get the 5 textures by web requests
        StartCoroutine(GetRequest(baseMapUrl, "_MainTex"));
        StartCoroutine(GetRequest(emissiveUrl, "_EmissionMap"));
        StartCoroutine(GetRequest(metallicRoughnessUrl, "_MetallicGlossMap"));
        StartCoroutine(GetRequest(normalUrl, "_BumpMap"));
        StartCoroutine(GetRequest(occlusionUrl, "_OcclusionMap"));

        // Wait until the requests are done
        while (slider.value < 1)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        slider.gameObject.SetActive(false);

        // Finally apply the material
        GetComponent<Renderer>().material = _material;
    }

    /// <summary>
    /// Make a web request at uri and apply the resulted texture to the material
    /// </summary>
    /// <param name="uri">The texture adress</param>
    /// <param name="textureName">The texture name to be set</param>
    /// <returns></returns>
    IEnumerator GetRequest(string uri, string textureName)
    {
        // The using statement ensures that the web request is disposed at the end
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
        {
            // Request and wait
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
                    Debug.Log("Received texture (" + textureName + "): " + webRequest.downloadHandler.text);
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
                    slider.value += 1f / 5f;
                    break;
            }
        }
    }
}
