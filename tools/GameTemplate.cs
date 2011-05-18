
	public class @CLASSNAME@ : Game
	{
		List <GameXmlDefinitionVariant> variants;

@VARIABLES_DEFINITION@

		public @CLASSNAME@ ()
		{
			List <GameXmlDefinitionVariant> variant;

			variants = new List <GameXmlDefinitionVariant> ();
		}

		public override string Name {
			get { return Catalog.GetString ("@NAME@"); }
		}

		public override string Question {
			get { return Catalog.GetString ("@_QUESTION@"); }
		}

		public override string Rationale {
			get { return StringExists ("@_RATIONALE@"); }
		}

		public override string Tip {
			get { return StringExists ("@TIP@");}
		}

		protected override void Initialize ()
		{

		}

		static string StringExists (string str)
		{
			if (string.IsNullOrEmpty (str))
				return string.Empty;

			return Catalog.GetString (str);
		}
	}

