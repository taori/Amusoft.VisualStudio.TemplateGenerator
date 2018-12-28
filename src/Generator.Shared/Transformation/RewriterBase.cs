using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Generator.Shared.Serialization;
using Generator.Shared.Utilities;
using NLog;

namespace Generator.Shared.Transformation
{
	public abstract class RewriterBase
	{
		private static readonly ILogger Log = LogManager.GetLogger(nameof(RewriterBase));

		protected void SaveTemplate(VsTemplate template, string templatePath)
		{
			Log.Info($"Saving template to \"{templatePath}\".");
			var serializer = new XmlSerializer(typeof(VsTemplate));

			using (var fileStream = new FileStream(templatePath, FileMode.Create))
			{
				using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
				{
					serializer.Serialize(streamWriter, template);
				}
			}

			if (!FileHelper.RemoveXmlMarker(templatePath))
				throw new Exception($"Failed to remove xml tag from template file.");
		}
	}
}