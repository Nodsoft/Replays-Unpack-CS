﻿using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Generators.SerializableEntity;

internal static class SerializableEntitySourceWriter
{
	public static string Generate(EntityToGenerate entity)
	{
		StringBuilder sb = new StringBuilder(SourceGenerationHelper.Header).AppendLine();
		sb.AppendLine("using System;");
		sb.AppendLine("using Nodsoft.WowsReplaysUnpack.Core.Entities;");
		sb.Append("namespace ").Append(entity.Namespace).AppendLine(";").AppendLine();

		sb.Append("public partial class ").Append(entity.ClassName).Append(": ISerializableEntity").AppendLine();
		sb.AppendLine("{").AppendLine();

		sb.AppendLine("  public void SetProperty(string name, object? value, int[] indexes)");
		sb.AppendLine("  {");

		sb.AppendLine("    switch (name)");
		sb.AppendLine("    {");

		foreach (PropertyData property in entity.Properties)
		{
			sb.Append($"      case \"{property.Path}\"");

			if (!property.IsClass)
			{
				sb.AppendLine($" when value is {property.Type} castedValue:");
			}
			else
			{
				sb.AppendLine(":");
			}

			sb.Append($"          {property.MappedPath}");

			if (property.IsAdd)
			{
				sb.AppendLine($".Add({(property.IsClass ? "new()" : "castedValue")});");
			}
			else if (property.IsClass)
			{
				sb.AppendLine(" = new();");
			}
			else
			{
				sb.AppendLine($" = castedValue;");
			}

			sb.AppendLine($"          break;");
		}

		sb.AppendLine("    }");

		sb.AppendLine("  }");

		sb.AppendLine("}");

		return sb.ToString();
	}
}