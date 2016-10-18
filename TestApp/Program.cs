using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHDLCodeGen;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			DefaultValues.CompanyName = "DNK Productions";
			DefaultValues.Developer = "Richard Dunkley";
			DefaultValues.UseTabs = true;
			DefaultValues.IncludeSubHeader = true;
			DefaultValues.FlowerBoxCharacter = '-';
			DefaultValues.NumCharactersPerLine = 130;
			DefaultValues.TabSize = 4;

			ModuleInfo info = new ModuleInfo(ArchitecturalType.RTL, new EntityInfo("TEST", "Test module to try out code gen.", "Additional Remarks"));
			info.Entity.Generics.Add(new GenericInfo("C_COUNT", "Count value", "std_logic_vector(15 downto 0)", "(others => '0')"));
			info.Entity.Generics.Add(new GenericInfo("C_DATA_WIDTH", "Data Width", "integer range 1 to 32", "8"));
			info.Entity.Ports.Add(new PortInfo("CLK", "Clock", PortDirection.In, "std_logic"));
			info.Entity.Ports.Add(new PortInfo("RESET", "Reset signal", PortDirection.In, "std_logic"));
			info.Entity.Ports.Add(new PortInfo("S_AXIS_TDATA", "Data In", PortDirection.In, "std_logic_vector(C_DATA_WIDTH-1 downto 0)"));
			info.Entity.Ports.Add(new PortInfo("S_AXIS_TVALID", "Valid In", PortDirection.In, "std_logic"));
			info.Entity.Ports.Add(new PortInfo("S_AXIS_TLAST", "Last In", PortDirection.In, "std_logic"));
			info.Entity.Ports.Add(new PortInfo("S_AXIS_TREADY", "Ready In", PortDirection.Out, "std_logic"));
			info.Entity.Ports.Add(new PortInfo("SERIAL_LINE", "Bidirectional serial line", PortDirection.InOut, "std_logic"));
			info.Entity.Remarks = "Remarks for the entity";

			info.AddUsing("IEEE.STD_LOGIC_1164.all");
			info.AddUsing("IEEE.NUMERIC_STD.all");

			info.DeclaredTypes.Add(new DeclarationInfo(DeclarationType.Constant, "ARRAY_SIZE", "integer range (8 to 16)", "Size of the array", "8"));
			info.DeclaredTypes.Add(new DeclarationInfo(DeclarationType.SubType, "ARRAY_SEGMENT", "unsigned(15 downto 0)", "Segment of the array"));
			info.DeclaredTypes.Add(new DeclarationInfo(DeclarationType.Type, "INC_ARRAY", "array(0 to ARRAY_SIZE-1) of ARRAY_SEGMENT", "Incrementing array"));
			info.DeclaredTypes.Add(new DeclarationInfo(DeclarationType.Constant, "DEFAULT_INC_ARRAY", "INC_ARRAY", "Default values of the incrementing array", "(others => (others => '0'))"));
			info.DeclaredTypes[3].Dependency.Add(info.DeclaredTypes[2]);
			info.DeclaredTypes[2].Dependency.Add(info.DeclaredTypes[0]);
			info.DeclaredTypes[2].Dependency.Add(info.DeclaredTypes[1]);

			info.Functions.Add(new FunctionInfo("SumUp", "unsigned", "Provides the summation of two numbers", "16-bit unsigned number containing the sum", "Some kind of remarks"));
			info.Functions[0].Parameters.Add(new ParameterInfo("number1", "unsigned(15 downto 0)", "First number to sum up"));
			info.Functions[0].Parameters.Add(new ParameterInfo("number2", "unsigned(15 downto 0)", "Second number to sum up"));
			info.Functions[0].Variables.Add(new VariableInfo("temp", "unsigned(15 downto 0)", "Temporary value to store the result"));
			info.Functions[0].CodeLines.Add("temp := number1 + number2;");
			info.Functions[0].CodeLines.Add("return temp;");

			info.Procedures.Add(new ProcedureInfo("SumpUpProc", "Sums up two number", "Provides sum stuff"));
			info.Procedures[0].Parameters.Add(new ProcedureParameterInfo("number1", PortDirection.In, "unsigned(15 downto 0)", "First number to sum up", ProcedureParameterType.Variable));
			info.Procedures[0].Parameters.Add(new ProcedureParameterInfo("number2", PortDirection.In, "unsigned(15 downto 0)", "Second number to sum up"));
			info.Procedures[0].Parameters.Add(new ProcedureParameterInfo("sum", PortDirection.Out, "unsigned(15 downto 0)", "Result of sum", ProcedureParameterType.Signal));
			info.Procedures[0].CodeLines.Add("sum <= number1 + number2;");

			ComponentInfo comp = new ComponentInfo("COUNT_UP", "Counts the values up");
			comp.Ports.Add(new SimplifiedPortInfo("CLK", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("ENABLE", PortDirection.In, "std_logic"));
			comp.Ports.Add(new SimplifiedPortInfo("COUNT", PortDirection.Out, "std_logic_vector(15 downto 0)"));

			info.SubModules.Add(new SubModule("COUNTER_MAIN", "Main counter", comp));
			info.SubModules[0].PortMap[comp.Ports[0]] = "CLK";
			info.SubModules[0].PortMap[comp.Ports[1]] = "RESET";
			info.SubModules[0].PortMap[comp.Ports[2]] = "open";

			FileInfo file = new FileInfo(info);
			file.WriteToFile(Environment.CurrentDirectory);
		}
	}
}
