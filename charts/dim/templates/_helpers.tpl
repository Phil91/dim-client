{{/*
Expand the name of the chart.
*/}}
{{- define "dim.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "dim.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "dim.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Determine secret name.
*/}}
{{- define "dim.secretName" -}}
{{- if .Values.existingSecret -}}
{{- .Values.existingSecret }}
{{- else -}}
{{- include "dim.fullname" . -}}
{{- end -}}
{{- end -}}

{{/*
Define secret name of postgres dependency.
*/}}
{{- define "dim.postgresSecretName" -}}
{{- printf "%s-%s" .Release.Name "issuer-postgres" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "dim.labels" -}}
helm.sh/chart: {{ include "dim.chart" . }}
{{ include "dim.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "dim.selectorLabels" -}}
app.kubernetes.io/name: {{ include "dim.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "dim.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "dim.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Determine database hostname for subchart
*/}}

{{- define "dim.postgresql.primary.fullname" -}}
{{- if eq .Values.postgresql.architecture "replication" }}
{{- printf "%s-primary" (include "dim.chart.name.postgresql.dependency" .) | trunc 63 | trimSuffix "-" -}}
{{- else -}}
    {{- include "dim.chart.name.postgresql.dependency" . -}}
{{- end -}}
{{- end -}}

{{- define "dim.postgresql.readReplica.fullname" -}}
{{- printf "%s-read" (include "dim.chart.name.postgresql.dependency" .) | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "dim.chart.name.postgresql.dependency" -}}
{{- if .Values.postgresql.fullnameOverride -}}
{{- .Values.postgresql.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default "postgresql" .Values.postgresql.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}
