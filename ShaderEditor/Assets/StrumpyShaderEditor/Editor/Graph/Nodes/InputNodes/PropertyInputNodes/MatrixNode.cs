using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrumpyShaderEditor
{
	[DataContract(Namespace = "http://strumpy.net/ShaderEditor/")]
	[NodeMetaData("Matrix", "Input", typeof(MatrixNode),"Specify a matrix input to be set by user scripting. Primarily for the vertex shader, this is required for use with VxM, MxM, and MxV, using this allows for arbitrary projections, rotates, and similar operations, but may eat most of the available interpolators, making it unsuitable below a shader model 3 target.")]
	public class MatrixNode : InputNode{
		private const string NodeName = "Matrix";
		
		[DataMember] private MatrixOutputChannel _matrix;
		
		public MatrixNode( )
		{
			Initialize();
		}
		
		public override sealed void Initialize ()
		{
			base.Initialize();
			_matrix = _matrix ?? new MatrixOutputChannel( 0, "Matrix" );
		}

		protected override ShaderProperty NewPropertyInstance ()
		{
			return new MatrixProperty();
		}
		
		//Beta3->Beta4 compatability.
		//TODO: Remove in B5
		protected override ShaderProperty InputPropertyFromOldInput()
		{
			var property = new MatrixProperty();
			property.PropertyName = _inputName;
			return property;
		}
		
		protected override bool OldPropertyConfigured()
		{
			return _inputName != null;
		}
		
		protected override void RemoveObsoleteInputConfig()
		{
			_inputName = null;
		}
		
		protected override IEnumerable<OutputChannel> GetOutputChannels()
		{
			return new List<OutputChannel> {_matrix};
		}
		
		public override IEnumerable<InputChannel> GetInputChannels()
		{
			return new List<InputChannel>();
		}
		
		public override string NodeTypeName
		{
			get{ return NodeName; }
		}

		protected override InputType GetFieldType()
		{
			return InputType.Matrix;
		}
		
		public override string GetExpression( uint channelId )
		{
			AssertOutputChannelExists( channelId );
			return GetFieldName();
		}
		
	}
}
