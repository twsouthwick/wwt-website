<%@ Page Language="C#" ContentType="text/plain" %> 

<%@ Import Namespace="WWT.Providers" %>
<%
	RequestProvider.Get<WebServiceProxyProvider>().Run(this);
%>
