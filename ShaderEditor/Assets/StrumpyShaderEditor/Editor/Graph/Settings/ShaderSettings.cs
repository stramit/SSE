using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class ShaderSettings
	{
		[DataMember] private EditorString _shaderName;
		[DataMember] private EditorString _shaderFallback;
		[DataMember] private ShaderTarget _shaderTarget;
		[DataMember] private CullMode _cullMode;
		[DataMember] private ZWrite _zWrite;
		[DataMember] private EditorBool _addShadow;
		[DataMember] private ZTest _zTest;
		[DataMember] private Queue _queue;
		[DataMember] private EditorInt _queueAdjust;
		[DataMember] private EditorBool _ignoreProjectors;
		
		[DataMember] private EditorBool _enableLOD;
		[DataMember] private EditorInt _lod;
		
		//3.2 variables
		[DataMember] private ExcludePath _excludePath;
		[DataMember] private EditorBool _dualForward;
		[DataMember] private EditorBool _fullForwardShadows;
		[DataMember] private EditorBool _softVegetation;
		[DataMember] private EditorBool _noAmbient;
		[DataMember] private EditorBool _noLightmap;
		[DataMember] private EditorBool _approxview;
		[DataMember] private EditorBool _halfasview;
		[DataMember] private EditorBool _noForwardAdd;
		[DataMember] private BlendingType _blending;
		[DataMember] private BlendingMode _srcBlend;
		[DataMember] private BlendingMode _dstBlend;
		
		[DataMember] private RenderType _renderType;
		[DataMember] private EditorString _renderTypeCustom;
		
		//Color Mask Channels
		[DataMember] private EditorBool _colorMaskR;
		[DataMember] private EditorBool _colorMaskG;
		[DataMember] private EditorBool _colorMaskB;
		[DataMember] private EditorBool _colorMaskA;
		
		//Fog Settings
		[DataMember] private EditorBool _fogModeOverride;
		[DataMember] private FogMode _fogMode;
		[DataMember] private EditorBool _fogColorOverride;
		[DataMember] private EditorColor _fogColor;
		[DataMember] private EditorBool _fogDensityOverride;
		[DataMember] private EditorFloat _fogDensity;
		[DataMember] private EditorFloat _fogNearLinear;
		[DataMember] private EditorFloat _fogFarLinear;
		
		public void Initialize()
		{
			_shaderName = _shaderName ?? "";
			_shaderFallback = _shaderFallback ?? "Diffuse";
			
			//Fog settings
			_fogModeOverride = _fogModeOverride ?? false;
			_fogColorOverride = _fogColorOverride ?? false;
			_fogColor = _fogColor ?? new Color( 1.0f, 1.0f, 1.0f, 1.0f);
			_fogDensityOverride = _fogDensityOverride ?? false;
			_fogDensity = _fogDensity ?? 0.5f;
			_fogNearLinear = _fogNearLinear ?? 0.0f;
			_fogFarLinear = _fogFarLinear ?? 1.0f;
			
			//RenderType
			_renderTypeCustom = _renderTypeCustom ?? new EditorString();
			
			_queueAdjust = _queueAdjust ?? 0;
			_enableLOD = _enableLOD ?? false;
			_lod = _lod ?? 100;
			_ignoreProjectors = _ignoreProjectors ?? false;
			
			//ColorMask
			_colorMaskR = _colorMaskR ?? true;
			_colorMaskG = _colorMaskG ?? true;
			_colorMaskB = _colorMaskB ?? true;
			_colorMaskA = _colorMaskA ?? true;
			
			_dualForward = _dualForward ?? false;
			_fullForwardShadows = _fullForwardShadows ?? false;
			_softVegetation = _softVegetation ?? false;
			_noAmbient = _noAmbient ?? false;
			_noLightmap = _noLightmap ?? false;
			_approxview = _approxview ?? false;
			_halfasview = _halfasview ?? false;
			_noForwardAdd = _noForwardAdd ?? false;
			_addShadow = _addShadow ?? false;
		}
		
		public bool SettingsValid()
		{
			if( string.IsNullOrEmpty( ShaderName ) )
			{
				return false;
			}
			return true;
		}
		
		public string ShaderName
		{
			get{ return _shaderName.Value.RemoveWhiteSpace(); }
		}
		
		public string FallBack
		{
			get{ return _shaderFallback.Value.RemoveWhiteSpace(); }
		}
		
		public ShaderTarget ShaderTarget
		{
			get{ return _shaderTarget; }
		}
		
		public CullMode CullMode
		{
			get{ return _cullMode; }
		}
		
		public ZWrite ZWrite
		{
			get{ return _zWrite; }
		}
		
		public ZTest ZTest
		{
			get{ return _zTest; }
		}
		
		public string ColorMask
		{
			get{
				//If we have a mask channel selected...
				if( _colorMaskR.Value || _colorMaskG.Value || _colorMaskB.Value || _colorMaskA.Value )
				{
					var mask = "";
					mask += _colorMaskR.Value ? "R" : "";
					mask += _colorMaskG.Value ? "G" : "";
					mask += _colorMaskB.Value ? "B" : "";
					mask += _colorMaskA.Value ? "A" : "";
					return mask;
				}
				return "0";
			}
		}
		
		public string FogSettings
		{
			get
			{
				var result ="Fog{\n";
				
				if( _fogModeOverride )
				{
					result += "Mode " + _fogMode.ToString() + "\n";
					
					if( _fogMode == FogMode.Linear )
					{
						result += "Range " + _fogNearLinear.Value + "," + _fogFarLinear.Value + "\n";
					}
				}
				if( _fogColorOverride )
				{
					result += "Color (" + _fogColor.R + ","+ _fogColor.G + ","+ _fogColor.B + ","+ _fogColor.A + ")" + "\n";
				}
				if( _fogDensityOverride )
				{
					result += "Density " + _fogDensity.Value + "\n";
				}
				result += "}\n";
				return result;
			}
		}
		
		public string BlendExpression
		{
			get{
				return _srcBlend.ToString() + " " + _dstBlend.ToString();
			}
		}
	
		public bool CustomBlendingEnabled()
		{
			return _blending == BlendingType.Custom;
		}
		
		public bool ShaderLODEnabled()
		{
			return _enableLOD;
		}
		
		public int ShaderLOD()
		{
			return _lod;
		}
		
		public bool AddShadowPass()
		{
			return _addShadow.Value;
		}
		
		public bool DualForwardLightmaps()
		{
			return _dualForward.Value;
		}
		
		public bool FullForwardShadows()
		{
			return _fullForwardShadows.Value;
		}
		
		public bool SoftVegetation()
		{
			return _softVegetation.Value;
		}
		
		public bool NoAmbient()
		{
			return _noAmbient.Value;
		}
		
		public bool NoLightmaps()
		{
			return _noLightmap.Value;
		}
		
		public bool ApproxViewVector()
		{
			return _approxview.Value;
		}
		
		public bool HalfVectorAsView()
		{
			return _halfasview.Value;
		}
		
		public bool NoForwardAdd()
		{
			return _noForwardAdd.Value;
		}
		
		public bool IgnoreProjectors()
		{
			return _ignoreProjectors.Value;
		}
		
		public bool ExcludeForward()
		{
			return _excludePath == ExcludePath.Forward;;
		}
		
		public bool ExcludeLightPrepass()
		{
			return _excludePath == ExcludePath.Prepass;
		}
		
		public string BlendingFlags()
		{
			var result = "";
			if( _blending == BlendingType.DecalAdd )
			{
				result += " alpha decal:add";
			}
			else if( _blending == BlendingType.DecalBlend )
			{
				result += " alpha decal:blend";
			}
			return result;
		}
		
		public string GetRenderQueue()
		{
			var val = _queueAdjust.Value;
			return val == 0 ? _queue.ToString() : val > 0 ? 
				_queue.ToString() + "+" + val.ToString() : 
				_queue.ToString() + val.ToString();
		}
		
		
		public string GetRenderType()
		{
			if( _renderType == RenderType.Custom )
			{
				return _renderTypeCustom;
			}
			else
			{
				return _renderType.ToString();
			}
		}
		
		public string Tags
		{
			get{
				var tags = "";
				
				tags += "\"Queue\"=\"" + GetRenderQueue () + "\"\n";
				tags += "\"IgnoreProjector\"=\"" + IgnoreProjectors() + "\"\n";
				tags += "\"RenderType\"=\"" + GetRenderType() + "\"\n";
				return tags;
			}
		}
		
		public string SurfaceFlags
		{
			get{
				var flags = "";
				
				if( AddShadowPass() )
				{
					flags += " addshadow";
				}
				
				if( ExcludeForward() )
				{
					flags += " exclude_path:forward";
				}
				else if( ExcludeLightPrepass() )
				{
					flags += " exclude_path:prepass";
				}
				
				if( DualForwardLightmaps() )
				{
					flags += " dualforward";
				}
				
				if( FullForwardShadows() )
				{
					flags += " fullforwardshadows";
				}
				
				if( SoftVegetation() )
				{
					flags += " softvegetation";
				}
				
				if( NoAmbient() )
				{
					flags += " noambient";
				}
				
				if( NoLightmaps() )
				{
					flags += " nolightmap";
				}
				
				if( NoForwardAdd() )
				{
					flags += " noforwardadd";
				}
				
				if( ApproxViewVector() )
				{
					flags += " approxview";
				}
				
				if( HalfVectorAsView() )
				{
					flags += " halfasview";
				}
				
				flags += BlendingFlags();
				return flags + " vertex:vert";
			}
		}
		
		public string Options
		{
			get{
				var options = "";
				options += "Cull " + CullMode.ToString() + "\n";
				options += "ZWrite " + ZWrite.ToString() + "\n";
				options += "ZTest " + ZTest.ToString() + "\n";
				options += "ColorMask " + ColorMask +"\n";
				if( ShaderLODEnabled() )
				{
					options += "LOD " + ShaderLOD() +"\n";
				}
				if( CustomBlendingEnabled() )
				{
					options += "Blend " + BlendExpression + "\n";
				}
				options += FogSettings;
				return options;
			}
		}
		
		public string Pragma
		{
			get{
				return "#pragma target " + ShaderTarget.TargetString();
			}
		}
		
		private bool _showQueueSettings = false;
		private bool _showCullingAndDepthSettings = false;
		private bool _showBlending = false;
		private bool _showColorAndLighting = false;
		private bool _showFogSettings = false;
		
		//property.Expanded =  EditorGUILayout.Foldout( property.Expanded, property.GetPropertyType().PropertyTypeString() );
		
		public void Draw()
		{
			var shaderNameContent = new GUIContent(
					"Shader Name",
					"Name for shader. Includes the hierarchy listing, that is, MyShaders/Shader will be in a folder called \"MyShaders\" in the shader selection dropdown. Also used when referring to fallback shaders.");
			
			var oldColor = GUI.color;
			if( string.IsNullOrEmpty(_shaderName ) )
			{
				GUI.color = Color.red;
			}
			_shaderName.Value = EditorGUILayout.TextField(shaderNameContent, _shaderName.Value);
			GUI.color = oldColor;
			
			var shaderFallbackContent = new GUIContent(
					"Shader Fallback",
					"Fallback shader to use in case this shader can not be used.");
			
			_shaderFallback.Value = EditorGUILayout.TextField( shaderFallbackContent, _shaderFallback.Value );
			
			var targetContent = new GUIContent("Shader Model","Requires more recent hardware to use the shader, but allows for more instructions, texture reads, and more input information.");
			_shaderTarget = (ShaderTarget)EditorGUILayout.EnumPopup( targetContent, _shaderTarget );
			
			var excludePathContent = new GUIContent("Exclude Path","Exclude a renderpath from shader generation");
			_excludePath = (ExcludePath)EditorGUILayout.EnumPopup( excludePathContent, _excludePath );
			
			GUILayout.Space(8);
			_showQueueSettings = EditorGUILayout.Foldout( _showQueueSettings, "Queue Settings" );
			if( _showQueueSettings )
			{
				var renderTypeContent = new GUIContent("Render Type","This is the rendertype tag inserted into the shader. Can be used for shader replace");
				_renderType = (RenderType)EditorGUILayout.EnumPopup( renderTypeContent, _renderType );
				
				if ( _renderType == RenderType.Custom ) 
					_renderTypeCustom.Value = EditorGUILayout.TextField( "Custom Type" ,_renderTypeCustom.Value );
				
				var queueContent = new GUIContent("Render Queue","The render queue that this material will be put in");
				_queue = (Queue)EditorGUILayout.EnumPopup( queueContent, _queue );
				
				var offContent = new GUIContent(
					"Queue Offset",
					"Offset for drawing. Used to ensure some things draw before or after others, it specifically is an offset from the given queue- That is to say, you won't have a transparent object draw before an opaque object (or similar) due to this offset.");
				_queueAdjust = EditorGUILayout.IntSlider(offContent, _queueAdjust.Value, -100, 100);
			}
			
			GUILayout.Space( 8 );
			_showCullingAndDepthSettings = EditorGUILayout.Foldout( _showCullingAndDepthSettings, "Culling and Depth Settings" );
			if( _showCullingAndDepthSettings )
			{
				var zWriteContent = new GUIContent("Write Depth","Depth is considered when testing other objects. Disable for certain effects, like letting other things draw over yourself, or for speed on most overlays.");
				_zWrite = (ZWrite)EditorGUILayout.EnumPopup( zWriteContent, _zWrite );
			
				var cullModeContent = new GUIContent("CullMode","Select back / forward to clip backwards facing polygons");
				_cullMode = (CullMode)EditorGUILayout.EnumPopup( cullModeContent, _cullMode );
			
				var zTestContent = new GUIContent("ZTest","Select Z-Test Value");
				_zTest = (ZTest)EditorGUILayout.EnumPopup( zTestContent, _zTest );
				
				var enableLODContent = new GUIContent("Enable LOD","Enable Shader LOD scaling");
				_enableLOD = EditorGUILayout.BeginToggleGroup( enableLODContent, _enableLOD );
				_lod = EditorGUILayout.IntSlider( "LOD", _lod, 0, 1000 );
				EditorGUILayout.EndToggleGroup();
			}
			
			GUILayout.Space( 8 );
			_showBlending = EditorGUILayout.Foldout( _showBlending, "Blending Settings" );
			if( _showBlending )
			{
				var blendingTypeContent = new GUIContent("Blend Type","Use a build in blend mode or a custom blend mode");
				_blending = (BlendingType)EditorGUILayout.EnumPopup( blendingTypeContent, _blending );
				
				if( CustomBlendingEnabled() )
				{
					var srcBlendContent = new GUIContent("Src Blend Mode","How the source channel of blending is used");
					_srcBlend = (BlendingMode)EditorGUILayout.EnumPopup( srcBlendContent, _srcBlend );
					
					var dstBlendContent = new GUIContent("Dst Blend Mode","How the destination channel of blending is used");
					_dstBlend = (BlendingMode)EditorGUILayout.EnumPopup( dstBlendContent, _dstBlend );
				}
			}
			
			GUILayout.Space( 8 );
			_showColorAndLighting = EditorGUILayout.Foldout( _showColorAndLighting, "Color And Lighting Settings" );
			if( _showColorAndLighting )
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label( "Color Mask:", GUILayout.ExpandWidth(false) );
				_colorMaskR.Value = EditorExtensions.ToggleButton( _colorMaskR.Value, "R","Mask R Channel");
				_colorMaskG.Value = EditorExtensions.ToggleButton( _colorMaskG.Value, "G","Mask G Channel");
				_colorMaskB.Value = EditorExtensions.ToggleButton( _colorMaskB.Value, "B","Mask B Channel");
				_colorMaskA.Value = EditorExtensions.ToggleButton( _colorMaskA.Value, "A","Mask A Channel");
				GUILayout.EndHorizontal();
				
				_dualForward = GUILayout.Toggle( _dualForward, new GUIContent( "Forward Dual Lightmaps","Use dual lightmaps in the forward rendering path" ));
				_fullForwardShadows = GUILayout.Toggle( _fullForwardShadows, new GUIContent( "Forward Full Shadows", "Support all shadow types in Forward rendering path." ));
				_softVegetation = GUILayout.Toggle( _softVegetation, new GUIContent( "Soft Vegetation", "Makes the surface shader only be rendered when Soft Vegetation is on." ));
				_noAmbient = GUILayout.Toggle( _noAmbient, new GUIContent( "No Ambient", "Do not apply any ambient lighting or spherical harmonics lights."));
				_noLightmap = GUILayout.Toggle( _noLightmap, new GUIContent( "No Lightmaps", "Disables lightmap support in this shader (makes a shader smaller)." ));
				_addShadow = GUILayout.Toggle( _addShadow, new GUIContent( "Advanced Shadow Pass", "Performs vertex transformations and clipping for the shadow pass, you need to use this if shadows do not display properly." ));
				
				_ignoreProjectors = GUILayout.Toggle( _ignoreProjectors, new GUIContent( "Ignore Projectors", "Ignores projector components, should be used if your doing custom vertex transformations or most transparency" ));
				_approxview = GUILayout.Toggle( _approxview, new GUIContent( "Approximate View", "Computes normalized view direction per-vertex instead of per-pixel, for shaders that need it. This is faster, but view direction is not entirely correct when camera gets close to surface." ));
				_halfasview = GUILayout.Toggle( _halfasview, new GUIContent( "Half As View", "Pass half-direction vector into the lighting function instead of view-direction. Half-direction will be computed and normalized per vertex. This is faster, but not entirely correct." ));
				_noForwardAdd = GUILayout.Toggle( _noForwardAdd, new GUIContent( "Disable Forward Add", "Disables Forward rendering additive pass. This makes the shader support one full directional light, with all other lights computed per-vertex/SH. Makes shaders smaller as well." ));
			}
			
			GUILayout.Space( 8 );
			_showFogSettings = EditorGUILayout.Foldout( _showFogSettings, "Fog Settings" );
			if( _showFogSettings )
			{
				_fogModeOverride = EditorGUILayout.BeginToggleGroup( "Fog Mode Override", _fogModeOverride );
				var fogModeContent = new GUIContent("Fog Mode","The type of fog to use");
				_fogMode = (FogMode)EditorGUILayout.EnumPopup( fogModeContent, _fogMode );
				
				if( _fogMode == FogMode.Linear )
				{
					_fogNearLinear.Value = EditorGUILayout.FloatField( "Near Linear Range:", _fogNearLinear );
					_fogFarLinear.Value = EditorGUILayout.FloatField( "Far Linear Range:", _fogFarLinear );
				}
				EditorGUILayout.EndToggleGroup();
				
				_fogColorOverride = EditorGUILayout.BeginToggleGroup( "Fog Color Override", _fogColorOverride );
				_fogColor.Value = EditorGUILayout.ColorField("Fog Color:", _fogColor );
				EditorGUILayout.EndToggleGroup();
				
				_fogDensityOverride = EditorGUILayout.BeginToggleGroup( "Fog Density Override", _fogDensityOverride );
				_fogDensity.Value = EditorGUILayout.FloatField( "Fog Density:", _fogDensity );
				EditorGUILayout.EndToggleGroup();
			}
		}
	}
}
