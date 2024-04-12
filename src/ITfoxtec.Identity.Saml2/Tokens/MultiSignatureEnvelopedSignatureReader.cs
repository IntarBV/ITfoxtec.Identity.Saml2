using System;
using System.Xml;
using Microsoft.IdentityModel.Xml;
using static Microsoft.IdentityModel.Logging.LogHelper;

namespace ITfoxtec.Identity.Saml2.Tokens
{
    public class MultiSignatureEnvelopedSignatureReader(XmlReader reader) : EnvelopedSignatureReader(reader)
    {
        private int _elementCount;

        public override bool Read()
        {
            bool result = true;
            bool completed = false;

            if ((NodeType == XmlNodeType.Element) && (!base.IsEmptyElement))
                _elementCount++;

            if (NodeType == XmlNodeType.EndElement)
            {
                _elementCount--;
                if (_elementCount == 0)
                {
                    OnEndOfRootElement();
                    completed = true;
                }
            }

            // If reading of an element will be completed in this pass, allow the InnerReader to record the signature position.
            if (completed && InnerReader is XmlTokenStreamReader xmlTokenStreamReader)
            {
                result = xmlTokenStreamReader.Read();
            }
            else
                result = InnerReader.Read();

            if (result
                && !completed
                && InnerReader.IsLocalName(XmlSignatureConstants.Elements.Signature)
                && InnerReader.IsNamespaceUri(XmlSignatureConstants.Namespace))
            {
                if (Signature == null)
                {
                    Signature = Serializer.ReadSignature(InnerReader);
                }
            }

            return result;
        }
    }
}
