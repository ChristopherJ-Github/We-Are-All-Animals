using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DepthTextImageEffect : ImageEffectBase
{
	private RenderTexture _depthNormalTex;
	public GameObject _depthNormalCam;
	private RenderTexture _silhouetteTexture;
	
	void CleanUpTextures() {
		if (_depthNormalTex) {
			RenderTexture.ReleaseTemporary(_depthNormalTex);
			_depthNormalTex = null;
		}
	}		
	
	void OnPreRender () 
	{
		if (!enabled || !gameObject.active) return;
		
		CleanUpTextures();
		
		_depthNormalTex = RenderTexture.GetTemporary((int)camera.pixelWidth,
		                                             (int)camera.pixelHeight, 16, RenderTextureFormat.ARGB32);

		if (!_depthNormalCam) {
			_depthNormalCam = new GameObject("SilhouetteCamera");
			_depthNormalCam.AddComponent<Camera>();
			_depthNormalCam.camera.enabled = false;
			_depthNormalCam.hideFlags = HideFlags.HideAndDontSave;
		}
		_depthNormalCam.camera.CopyFrom(camera);
		_depthNormalCam.camera.backgroundColor = new Color(0,0,0,0);
		_depthNormalCam.camera.clearFlags = CameraClearFlags.SolidColor;
		
		//_depthNormalCam.camera.targetTexture = _silhouetteTexture;
		_depthNormalCam.camera.RenderWithShader(
			Shader.Find("Hidden/Camera-DepthNormalTexture"), null);
		_depthNormalCam.camera.depthTextureMode |= DepthTextureMode.Depth;
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetTexture("_DepthNormal", _depthNormalTex);
		ImageEffects.BlitWithMaterial(material, source, destination);
		CleanUpTextures();
	}		
	
	new void OnDisable () {
		if (_depthNormalCam) {
			DestroyImmediate(_depthNormalCam);
		}
		base.OnDisable();
	}
}