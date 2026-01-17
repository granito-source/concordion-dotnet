/*
 * Copyright 2026 Alexei Yashkov
 * Copyright 2010-2015 concordion.org
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Reflection;
using Concordion.Api.Extension;
using Concordion.Internal.Util;

namespace Concordion.Internal.Extension;

public class ExtensionLoader(SpecificationConfig configuration) {
    private static List<ConcordionExtension> GetExtensionsForFixture(
        object fixture)
    {
        var extensions = new List<ConcordionExtension>();

        foreach (var type in GetClassHierarchyParentFirst(fixture.GetType())) {
            extensions.AddRange(GetExtensionsFromFieldAttributes(fixture, type));
            extensions.AddRange(GetExtensionsFromClassAttributes(type));
        }

        return extensions;
    }

    private static List<ConcordionExtension> GetExtensionsFromFieldAttributes(
        object fixture, Type fixtureType)
    {
        var extensions = new List<ConcordionExtension>();
        var fieldInfos = fixtureType.GetFields(
            BindingFlags.Public |
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.Static);

        foreach (var fieldInfo in fieldInfos) {
            if (!HasAttribute(fieldInfo, typeof(ExtensionAttribute), false))
                continue;

            var extension = fieldInfo.GetValue(fixture) as ConcordionExtension;

            Check.NotNull(extension, $"Extension field '{fieldInfo.Name}' must be non-null");

            extensions.Add(extension);
        }

        return extensions;
    }

    private static List<ConcordionExtension> GetExtensionsFromClassAttributes(
        Type fixtureType)
    {
        return HasAttribute(fixtureType, typeof(ExtensionsAttribute), false) ? (
            from attribute in fixtureType
                .GetCustomAttributes(typeof(ExtensionsAttribute), false)
            select attribute as ExtensionsAttribute
            into extensionsAttribute
            from extensionType in extensionsAttribute.ExtensionTypes
            select CreateConcordionExtension(
                extensionType.Assembly.GetName().Name,
                extensionType.FullName)
        ).ToList() : [];
    }

    private static List<Type> GetClassHierarchyParentFirst(Type fixtureType)
    {
        var fixtureTypes = new List<Type>();
        var current = fixtureType;

        while (current != null && current != typeof(object)) {
            fixtureTypes.Add(current);
            current = current.BaseType;
        }

        fixtureTypes.Reverse();

        return fixtureTypes;
    }

    private static ConcordionExtension CreateConcordionExtension(
        string? assembly, string? type)
    {
        Check.NotNull(assembly, "assembly may not be null");
        Check.NotNull(type, "type may not be null");

        var instance = Activator.CreateInstance(assembly, type)?.Unwrap();

        return instance switch {
            ConcordionExtension extension => extension,
            ConcordionExtensionFactory factory => factory.CreateExtension(),
            _ => throw new InvalidCastException(
                $"Extension {type} must implement {typeof(ConcordionExtension)} or {typeof(ConcordionExtensionFactory)}")
        };
    }

    private static bool HasAttribute(MemberInfo memberInfo,
        Type attributeType, bool inherit)
    {
        return memberInfo
            .GetCustomAttributes(attributeType, inherit)
            .Any(attribute => attribute.GetType() == attributeType);
    }

    public void AddExtensions(object fixture,
        ConcordionExtender concordionExtender)
    {
        foreach (var concordionExtension in GetExtensionsFromConfiguration())
            concordionExtension.AddTo(concordionExtender);

        foreach (var concordionExtension in GetExtensionsForFixture(fixture))
            concordionExtension.AddTo(concordionExtender);
    }

    private List<ConcordionExtension> GetExtensionsFromConfiguration()
    {
        return (
            from extension in configuration.ConcordionExtensions
            select CreateConcordionExtension(extension.Value, extension.Key)
        ).ToList();
    }
}
