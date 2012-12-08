using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	public class ShaderMasterNode : RootNode {
		private const string NodeName = "Master";
		
		[DataMember] private Float4InputChannel _albedo;
		[DataMember] private Float4InputChannel _normal;
		[DataMember] private Float4InputChannel _emission;
		[DataMember] private Float4InputChannel _specular;
		[DataMember] private Float4InputChannel _gloss;
		[DataMember] private Float4InputChannel _alpha;
		[DataMember] private Float4InputChannel _custom;
		
		[DataMember] private Float4InputChannel _clip;
		
		public ShaderMasterNode( )
		{
			Initialize(); 
		}
		
		public override sealed void Initialize ()
		{
			
			_albedo = _albedo ?? new Float4InputChannel( 0, "Diffuse", Vector4.zero );
			_normal = _normal ?? new Float4InputChannel( 1, "Normal", new Vector4( 0.0f, 0.0f, 1.0f, 1.0f ) );
			_emission = _emission ?? new Float4InputChannel( 2, "Emission", Vector4.zero );
			_specular = _specular ?? new Float4InputChannel( 3, "Specular", Vector4.zero );
			_gloss = _gloss ?? new Float4InputChannel( 4, "Gloss", Vector4.zero );
			_alpha = _alpha ?? new Float4InputChannel( 5, "Alpha", Vector4.one );
			_clip = _clip ?? new Float4InputChannel( 6, "Clip", Vector4.one );
			_custom = _custom ?? new Float4InputChannel( 7, "Custom", Vector4.zero );
			
			_specular.DisplayName = "Gloss";
			_gloss.DisplayName = "Specular";
			_albedo.DisplayName = "Diffuse";
		}

		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel>();
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel> {_albedo, _normal, _emission, _specular, _gloss, _alpha, _custom, _clip };
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}
		
		public override string GetExpression( uint channelId )
		{
			Debug.LogError( "Can not get node based expression from the master node" );
			return "";
		}
		
		public string GetAdditionalFields()
		{
			var albedoInput = _albedo.ChannelInput( this );
			var normalInput = _normal.ChannelInput( this );
			var emissionInput = _emission.ChannelInput( this );
			var specularInput = _specular.ChannelInput( this );
			var glossInput = _gloss.ChannelInput( this );
			var alphaInput = _alpha.ChannelInput( this );
			var customInput = _custom.ChannelInput( this );
			var clipInput = _clip.ChannelInput( this );
			
			var result = albedoInput.AdditionalFields;
			result += normalInput.AdditionalFields;
			result += emissionInput.AdditionalFields;
			result += specularInput.AdditionalFields;
			result += glossInput.AdditionalFields;
			result += alphaInput.AdditionalFields;
			result += customInput.AdditionalFields;
			result += clipInput.AdditionalFields;
			return result;
		}
		
		public bool AlbedoConnected()
		{
			return _albedo.IncomingConnection != null;
		}
		public string GetAlbedoExpression()
		{
			return _albedo.ChannelInput( this ).QueryResult;
		}
		
		public bool NormalConnected()
		{
			return _normal.IncomingConnection != null;
		}
		public string GetNormalExpression()
		{
			return _normal.ChannelInput( this ).QueryResult;
		}
		
		public bool EmissionConnected()
		{
			return _emission.IncomingConnection != null;
		}
		public string GetEmissionExpression()
		{
			return _emission.ChannelInput( this ).QueryResult;
		}
		
		public bool SpecularConnected()
		{
			return _specular.IncomingConnection != null;
		}
		public string GetSpecularExpression()
		{
			return _specular.ChannelInput( this ).QueryResult;
		}
		
		public bool GlossConnected()
		{
			return _gloss.IncomingConnection != null;
		}
		public string GetGlossExpression()
		{
			return _gloss.ChannelInput( this ).QueryResult;
		}
		
		public bool AlphaConnected()
		{
			return _alpha.IncomingConnection != null;
		}
		public string GetAlphaExpression()
		{
			return _alpha.ChannelInput( this ).QueryResult;
		}
		
		public bool CustomConnected()
		{
			return _custom.IncomingConnection != null;
		}
		public string GetCustomExpression()
		{
			return _custom.ChannelInput( this ).QueryResult;
		}

		public bool ClipConnected()
		{
			return _clip.IncomingConnection != null;
		}
		public string GetClipExpression()
		{
			return _clip.ChannelInput( this ).QueryResult;
		}
	}
}
