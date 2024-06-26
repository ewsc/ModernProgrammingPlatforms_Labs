﻿using System.Reflection;

namespace LabTres
{
    public class TEST_CLASS_NAME
    {
        public String TEST_STRING;
        public int TEST_INT;
        public float TEST_FLOAT;

        public void TEST_VOID()
        {
            return;
        }
    }
    
    public class AssemblyInfo
    {
        public string Name { get; set; }
        public List<NamespaceInfo> Namespaces { get; set; }

        public AssemblyInfo(string assemblyPath)
        {
            Namespaces = new List<NamespaceInfo>();
            LoadAssembly(assemblyPath);
        }
        

        private void LoadAssembly(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Name = assembly.FullName;


            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                string namespaceName = type.Namespace ?? "<Global>";
                NamespaceInfo namespaceInfo = GetOrCreateNamespace(namespaceName);
                TypeInfo typeInfo = new TypeInfo(type.Name);

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (FieldInfo field in fields)
                {
                    MemberInfo memberInfo = new MemberInfo(field.Name, field.FieldType.Name, field.FieldType.FullName);
                    typeInfo.Members.Add(memberInfo);
                }

                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (PropertyInfo property in properties)
                {
                    MemberInfo memberInfo = new MemberInfo(property.Name, property.PropertyType.Name, property.PropertyType.FullName);
                    typeInfo.Members.Add(memberInfo);
                }

                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (MethodInfo method in methods)
                {
                    string returnType = method.ReturnType.Name;
                    string methodName = method.Name;
                    string parameterList = GetParameterList(method.GetParameters());
                    string signature = $"{returnType} {methodName}({parameterList})";
                    MemberInfo memberInfo = new MemberInfo(methodName, signature, returnType);
                    typeInfo.Members.Add(memberInfo);
                }

                namespaceInfo.Types.Add(typeInfo);
            }
        }

        public NamespaceInfo GetOrCreateNamespace(string namespaceName)
        {
            foreach (NamespaceInfo namespaceInfo in Namespaces)
            {
                if (namespaceInfo.Name == namespaceName)
                    return namespaceInfo;
            }

            NamespaceInfo newNamespaceInfo = new NamespaceInfo(namespaceName);
            Namespaces.Add(newNamespaceInfo);
            return newNamespaceInfo;
        }

        public string GetParameterList(ParameterInfo[] parameters)
        {
            List<string> parameterList = new List<string>();
            foreach (ParameterInfo parameter in parameters)
            {
                string parameterString = $"{parameter.ParameterType.Name} {parameter.Name}";
                parameterList.Add(parameterString);
            }
            return string.Join(", ", parameterList);
        }
    }

    public class NamespaceInfo
    {
        public string Name { get; set; }
        public List<TypeInfo> Types { get; set; }

        public NamespaceInfo(string name)
        {
            Name = name;
            Types = new List<TypeInfo>();
        }
    }

    public class TypeInfo
    {
        public string Name { get; set; }
        public List<MemberInfo> Members { get; set; }

        public TypeInfo(string name)
        {
            Name = name;
            Members = new List<MemberInfo>();
        }
    }

    public class MemberInfo
    {
        public string Name { get; set; }
        public string Signature { get; set; }
        public string Type { get; set; }

        public MemberInfo(string name, string signature, string type)
        {
            Name = name;
            Signature = signature;
            Type = type;
        }
    }
}